using ApacheNet.Models;
using CustomLogger;

namespace ApacheNet
{
    public static class ApachePlugin
    {
        public static async Task<bool> ProcessPlugin(ApacheContext ctx)
        {
            var sent = false;

            if (!ApacheNetServerConfiguration.plugins.IsEmpty)
            {
                foreach (var plugin in ApacheNetServerConfiguration.plugins.Values)
                {
                    try
                    {
                        var objReturn = plugin.ProcessPluginMessage(ctx);
                        if (objReturn != null)
                        {
                            if (objReturn is bool v)
                                sent = v;
                            else if (objReturn is Task<object?> t)
                            {
                                var taskResult = await t.ConfigureAwait(false);
                                if (taskResult != null && taskResult is bool v0)
                                    sent = v0;
                            }
                        }
                        // Backward compatibility path.
                        else
                        {
                            objReturn = plugin.ProcessPluginMessage(ctx.Context);
                            if (objReturn != null)
                            {
                                if (objReturn is bool v)
                                    sent = v;
                                else if (objReturn is Task<object?> t)
                                {
                                    var taskResult = await t.ConfigureAwait(false);
                                    if (taskResult != null && taskResult is bool v0)
                                        sent = v0;
                                }
                            }
                        }
                        if (sent)
                            break;
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(
                            $"[{(ctx.Secure ? "HTTPS" : "HTTP")}] - Plugin {plugin.GetHashCode()} thrown an assertion: {ex}"
                        );
                    }
                }
            }

            return sent;
        }
    }
}
