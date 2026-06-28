using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class ParametersWithContext : ICipherParameters
    {
        public static ParametersWithContext Create<TState>(
            ICipherParameters parameters,
            int contextLength,
            TState state,
            System.Buffers.SpanAction<byte, TState> action
        )
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            ParametersWithContext result = new ParametersWithContext(parameters, contextLength);
            action(result.m_context, state);
            return result;
        }

        private readonly ICipherParameters m_parameters;
        private readonly byte[] m_context;

        public ParametersWithContext(ICipherParameters parameters, byte[] context)
        {
            // NOTE: 'parameters' may be null to imply key re-use
            m_parameters = parameters;
            m_context = Arrays.CopyBuffer(context);
        }

        public ParametersWithContext(
            ICipherParameters parameters,
            byte[] context,
            int contextOff,
            int contextLen
        )
        {
            // NOTE: 'parameters' may be null to imply key re-use
            m_parameters = parameters;
            m_context = Arrays.CopySegment(context, contextOff, contextLen);
        }

        public ParametersWithContext(ICipherParameters parameters, ReadOnlySpan<byte> context)
        {
            // NOTE: 'parameters' may be null to imply key re-use
            m_parameters = parameters;
            m_context = context.ToArray();
        }

        private ParametersWithContext(ICipherParameters parameters, int contextLength)
        {
            // NOTE: 'parameters' may be null to imply key re-use
            m_parameters = parameters;
            m_context = Arrays.CreateBuffer<byte>(contextLength);
        }

        public int ContextLength => m_context.Length;

        public void CopyContextTo(byte[] buf, int off, int len) =>
            Arrays.CopyBufferToSegment(m_context, buf, off, len);

        public byte[] GetContext() => Arrays.InternalCopyBuffer(m_context);

        internal ReadOnlySpan<byte> InternalContext => m_context;

        public ICipherParameters Parameters => m_parameters;
    }
}
