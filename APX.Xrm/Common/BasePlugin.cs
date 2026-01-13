using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Reflection;

namespace APX.Xrm
{
    public abstract class BasePlugin : IPlugin
    {
        public abstract string[] ExpectedMessages { get; }
        public abstract string[] ExpectedEntities { get; }
        protected string secureConfig;
        protected string unsecureConfig;
        public BasePlugin()
        {
            secureConfig = string.Empty;
            unsecureConfig = string.Empty;
        }

        public BasePlugin(string unsecureConfig, string secureConfig)
        {
            this.unsecureConfig = unsecureConfig;
            this.secureConfig = secureConfig;
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            var ctx = new PluginContext(serviceProvider);
            var ass = Assembly.GetExecutingAssembly().FullName;
            var version = ass.Split(',')[1].Split('=')[1];
            ctx.Trace($"Plugin Instantiated: {GetType()}. Version: {version}");
            ctx.Trace($"Primary Record: {ctx.Context.PrimaryEntityName} {ctx.Context.PrimaryEntityId}");
            ctx.Trace($"Message: {ctx.Context.MessageName}. Stage: {ctx.Context.Stage}. Depth: {ctx.Context.Depth}");
            ctx.Trace($"User ID: {ctx.Context.UserId}");
            ctx.Trace($"In Transaction: {ctx.Context.IsInTransaction}");
            ctx.SecureConfig = secureConfig;
            ctx.UnSecureConfig = unsecureConfig;

            if (ExpectedMessages?.Length > 0 && !ExpectedMessages.Contains(ctx.Context.MessageName))
            {
                ctx.Trace($"Wrong Message: {ctx.Context.MessageName}");
                return;
            }

            if (ExpectedEntities?.Length > 0 && !ExpectedEntities.Contains(ctx.Context.PrimaryEntityName))
            {
                ctx.Trace($"Wrong Entity: {ctx.Context.PrimaryEntityName}");
                return;
            }
            ExecutePlugin(ctx);
        }
        public abstract void ExecutePlugin(PluginContext pluginCtx);

    }
}
