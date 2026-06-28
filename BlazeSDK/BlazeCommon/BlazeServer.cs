using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CustomLogger;

namespace BlazeCommon
{
    public class BlazeServer(BlazeServerConfiguration settings)
        : ProtoFireServer(settings.Name, settings.LocalEP, settings.Certificate, settings.ForceSsl)
    {
        public BlazeServerConfiguration Configuration { get; } = settings;

        private readonly ConcurrentDictionary<
            ProtoFireConnection,
            BlazeServerConnection
        > _connections = new();

        public bool AddComponent<TComponent>()
            where TComponent : IBlazeServerComponent, new()
        {
            return Configuration.AddComponent<TComponent>();
        }

        public bool AddComponent(
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                Type componentType
        )
        {
            return !typeof(IBlazeServerComponent).IsAssignableFrom(componentType)
                ? throw new ArgumentException(
                    "Type must implement IBlazeServerComponent",
                    nameof(componentType)
                )
                : Activator.CreateInstance(componentType) is IBlazeServerComponent component
                    && Configuration.AddComponent(component);
        }

        public bool RemoveComponent(ushort componentId, out IBlazeServerComponent? component)
        {
            return Configuration.RemoveComponent(componentId, out component);
        }

        public IBlazeServerComponent? GetComponent(ushort componentId)
        {
            return Configuration.GetComponent(componentId);
        }

        BlazeServerConnection GetBlazeConnection(ProtoFireConnection connection)
        {
            return _connections.GetOrAdd(
                connection,
                (c) =>
                {
                    return new BlazeServerConnection(c, Configuration);
                }
            );
        }

        public override Task OnProtoFireConnectAsync(ProtoFireConnection connection)
        {
            Configuration.OnNewConnection?.Invoke(GetBlazeConnection(connection));
            return Task.CompletedTask;
        }

        public override Task OnProtoFireDisconnectAsync(ProtoFireConnection connection)
        {
            if (_connections.TryRemove(connection, out var connectionInfo))
                Configuration.OnDisconnected?.Invoke(connectionInfo);
            return Task.CompletedTask;
        }

        public override Task OnProtoFireErrorAsync(
            ProtoFireConnection connection,
            Exception exception
        )
        {
            OnProtoFireError(connection, exception);
            return Task.CompletedTask;
        }

        private void OnProtoFireError(ProtoFireConnection connection, Exception exception)
        {
            LoggerAccessor.LogError(
                $"[BlazeServer] - ProtoFireError occured (Exception: {exception})"
            );
            Configuration.OnError?.Invoke(GetBlazeConnection(connection), exception);
        }

        IBlazePacket DecodePacket(ProtoFirePacket packet)
        {
            var frame = packet.Frame;
            var component = Configuration.GetComponent(frame.Component);
            if (component == null)
                return packet.Decode(typeof(NullStruct), Configuration.Decoder);
            var type = frame.MsgType switch
            {
                FireFrame.MessageType.MESSAGE => component.GetCommandRequestType(frame.Command),
                FireFrame.MessageType.REPLY => component.GetCommandResponseType(frame.Command),
                FireFrame.MessageType.NOTIFICATION => component.GetNotificationType(frame.Command),
                FireFrame.MessageType.ERROR_REPLY => component.GetCommandErrorResponseType(
                    frame.Command
                ),
                _ => typeof(NullStruct),
            };
            type ??= typeof(NullStruct);
            return packet.Decode(type, Configuration.Decoder);
        }

        Task SendBlazePacket(
            ProtoFireConnection connection,
            IBlazeComponent? component,
            IBlazePacket packet
        )
        {
            BlazeUtils.LogPacket(component, packet, false);
            return connection.SendAsync(packet.ToProtoFirePacket(Configuration.Encoder));
        }

        static IBlazePacket GetErrorResponse(
            IBlazePacket requestPacket,
            BlazeRpcException exception
        )
        {
            return exception.ErrorResponse != null
                ? requestPacket.CreateResponsePacket(exception.ErrorResponse, exception.ErrorCode)
                : requestPacket.CreateResponsePacket(exception.ErrorCode);
        }

        //TODO: Rewrite this method
        public override async Task OnProtoFirePacketReceivedAsync(
            ProtoFireConnection connection,
            ProtoFirePacket packet
        )
        {
            var frame = packet.Frame;
            var blazePacket = DecodePacket(packet);
            var component = Configuration.GetComponent(frame.Component);
            BlazeUtils.LogPacket(component, blazePacket, true);

            if (frame.MsgType != FireFrame.MessageType.MESSAGE)
            {
                LoggerAccessor.LogError(
                    $"[BlazeServer] - Connection({connection.ID}) message with type {frame.MsgType} not handled!"
                );
                return;
            }

            IBlazePacket response;
            if (component == null)
            {
                response = blazePacket.CreateResponsePacket(
                    new NullStruct(),
                    Configuration.ComponentNotFoundErrorCode
                );
                await SendBlazePacket(connection, component, response).ConfigureAwait(false);
                return;
            }

            var commandInfo = component.GetBlazeCommandInfo(frame.Command);
            if (commandInfo == null)
            {
                response = blazePacket.CreateResponsePacket(
                    new NullStruct(),
                    Configuration.CommandNotFoundErrorCode
                );
                await SendBlazePacket(connection, component, response).ConfigureAwait(false);
                return;
            }

            var unhandled = false;
            var blazeConnection = GetBlazeConnection(connection);
            //marking that blaze connection is busy with some kind of request
            await blazeConnection.IsBusyLock.EnterAsync().ConfigureAwait(false);
            try
            {
                var context = new BlazeRpcContext(
                    blazeConnection,
                    frame.FullErrorCode,
                    frame.MsgNum,
                    frame.UserIndex,
                    frame.Context
                );
                var responseObj = await commandInfo
                    .InvokeAsync(blazePacket.DataObj, context)
                    .ConfigureAwait(false);
                response = blazePacket.CreateResponsePacket(responseObj);
                context = null;
            }
            catch (Exception exception)
            {
                if (exception is BlazeRpcException rpcException)
                {
                    if (
                        rpcException.ErrorCode == Configuration.CommandNotFoundErrorCode
                        || rpcException.ErrorCode == Configuration.ComponentNotFoundErrorCode
                    )
                        unhandled = true;

                    if (rpcException.InnerException != null)
                        OnProtoFireError(connection, rpcException.InnerException);

                    response = GetErrorResponse(blazePacket, rpcException);
                }
                else if (
                    exception is TargetInvocationException targException
                    && targException.InnerException is BlazeRpcException rpcException2
                )
                {
                    if (
                        rpcException2.ErrorCode == Configuration.CommandNotFoundErrorCode
                        || rpcException2.ErrorCode == Configuration.ComponentNotFoundErrorCode
                    )
                        unhandled = true;

                    if (rpcException2.InnerException != null)
                        OnProtoFireError(connection, rpcException2.InnerException);

                    response = GetErrorResponse(blazePacket, rpcException2);
                }
                else
                {
                    response = blazePacket.CreateResponsePacket(
                        new NullStruct(),
                        Configuration.ErrSystemErrorCode
                    );
                    OnProtoFireError(connection, exception);
                }
            }

            try
            {
                Configuration.OnRequest?.Invoke(blazeConnection, packet, unhandled);
            }
            catch { }

            await SendBlazePacket(connection, component, response).ConfigureAwait(false);
            blazeConnection.IsBusyLock.Exit();
        }
    }
}
