using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace APX.Xrm
{
    public class PluginContext : ITracingService
    {
        private Lazy<IOrganizationService> service;
        private Lazy<IPluginExecutionContext> context;
        private IOrganizationServiceFactory serviceFactory;
        private Lazy<ITracingService> tracer;

        public IOrganizationService Service => service.Value;
        public IPluginExecutionContext Context => context.Value;
        public ITracingService Tracer => tracer.Value;

        public Entity Target => new Lazy<Entity>(() => getTarget()).Value;

        private Entity getTarget()
        {
            return Context.InputParameters.Contains("Target") && Context.InputParameters["Target"] is Entity result ? result : null;
        }
        public Entity PreImage => new Lazy<Entity>(() => getImage(Context.PreEntityImages)).Value;
        public Entity PostImage => new Lazy<Entity>(() => getImage(Context.PostEntityImages)).Value;

        private Entity getImage(EntityImageCollection images, bool errorIfMissing = true)
        {
            Entity result = images.Any() ? images.Values.First() : null;
            if (errorIfMissing && result == null)
            {
                throw new InvalidPluginExecutionException("Entity Image collection is empty.");
            }
            return result;
        }
        public string SecureConfig { get; set; }
        public string UnSecureConfig { get; set; }
        public PluginContext(IServiceProvider serviceProvider)
        {
            tracer = new Lazy<ITracingService>(() => (ITracingService)serviceProvider.GetService(typeof(ITracingService)));
            context = new Lazy<IPluginExecutionContext>(() => (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext)));
            serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            service = new Lazy<IOrganizationService>(() => serviceFactory.CreateOrganizationService(context.Value.UserId));
          
        }

        public void Trace(string format, params object[] args)
        {
            var msg = string.Format(format, args);
            Tracer.Trace("{0} {1}", DateTime.Now.ToString("HH:mm:ss:fff"), msg);
        }
    }
}
