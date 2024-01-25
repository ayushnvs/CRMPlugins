using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace Plugins
{
    public class HandleFullName : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity pipeline = (Entity)context.InputParameters["Target"];

                // Obtain the IOrganizationService instance which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    String firstName = String.Empty;
                    if (pipeline.Attributes.Contains("pra_firstname"))
                    {
                        firstName = pipeline.Attributes["pra_firstname"].ToString();
                    }

                    String lastName = String.Empty;
                    if (pipeline.Attributes.Contains("pra_lastname"))
                    {
                        lastName = pipeline.Attributes["pra_lastname"].ToString();
                    }

                    String fullName = lastName;
                    if (firstName != String.Empty)
                    {
                        fullName = firstName + " " + lastName;
                    }

                    pipeline.Attributes.Add("pra_fullname", fullName);

                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
    public class UpdateFullName : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity pipeline = (Entity)context.InputParameters["Target"];
                Entity pipelineImage = (Entity)context.PreEntityImages["FullNameUpdate"];

                // Obtain the IOrganizationService instance which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    String firstName = String.Empty;
                    if (pipeline.Attributes.Contains("pra_firstname"))
                    {
                        firstName = pipeline.Attributes["pra_firstname"].ToString();
                    } 
                    else if (pipelineImage.Attributes.Contains("pra_firstname"))
                    {
                        firstName = pipelineImage.Attributes["pra_firstname"].ToString();
                    }

                    String lastName = String.Empty;
                    if (pipeline.Attributes.Contains("pra_lastname"))
                    {
                        lastName = pipeline.Attributes["pra_lastname"].ToString();
                    } 
                    else if (pipelineImage.Attributes.Contains("pra_lastname"))
                    {
                        lastName = pipelineImage.Attributes["pra_lastname"].ToString();
                    }

                    String fullName = lastName;
                    if (firstName != String.Empty)
                    {
                        fullName = firstName + " " + lastName;
                    }

                    pipeline.Attributes.Add("pra_fullname", fullName);

                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
