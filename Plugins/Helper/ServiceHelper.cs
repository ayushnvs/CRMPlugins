using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace MyCompany.Practice.Plugins.Helper
{
    public class ServiceHelper
    {
        public static ITracingService GetTracingService(IServiceProvider serviceProvider)
        {
            return (ITracingService)serviceProvider.GetService(typeof(ITracingService));
        }

        public static (ITracingService, IPluginExecutionContext) GetTracingService(IServiceProvider serviceProvider, out IPluginExecutionContext context)
        {
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            return (GetTracingService(serviceProvider), context);
        }

        public static (ITracingService, IPluginExecutionContext, IOrganizationService) GetTracingService(IServiceProvider serviceProvider, out IPluginExecutionContext context, out IOrganizationService service)
        {
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            service = serviceFactory.CreateOrganizationService(context.UserId);

            return (GetTracingService(serviceProvider), context, service);
        }
    }
}
