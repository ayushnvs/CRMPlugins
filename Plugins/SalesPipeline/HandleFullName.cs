using Microsoft.Xrm.Sdk;
using MyCompany.Practice.Plugins.Helper;
using System;
using System.ServiceModel;

namespace MyCompany.Practice.Plugins.SalesPipeline
{
    public class HandleFullName : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Getting Services using Helper
            var (tracingService, context) = ServiceHelper.GetTracingService(serviceProvider, out var executionContext);

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entPipeline = (Entity)context.InputParameters["Target"];

                try
                {
                    String firstName = String.Empty;
                    if (entPipeline.Attributes.Contains("pra_firstname"))
                    {
                        firstName = entPipeline.Attributes["pra_firstname"].ToString();
                    }

                    String lastName = String.Empty;
                    if (entPipeline.Attributes.Contains("pra_lastname"))
                    {
                        lastName = entPipeline.Attributes["pra_lastname"].ToString();
                    }

                    String fullName = lastName;
                    if (firstName != String.Empty)
                    {
                        fullName = firstName + " " + lastName;
                    }

                    entPipeline.Attributes.Add("pra_fullname", fullName);

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
            // Getting Services using Helper
            var (tracingService, context) = ServiceHelper.GetTracingService(serviceProvider, out var executionContext);
  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {  
                Entity entPipeline = (Entity)context.InputParameters["Target"];
                Entity entPipelineImage = (context.PreEntityImages.Contains("FullNameUpdate") && context.PreEntityImages["FullNameUpdate"] != null)
                    ? (Entity)context.PreEntityImages["FullNameUpdate"]
                    : throw new InvalidPluginExecutionException("Pre entity image is not registered.");

                try
                {
                    String firstName = String.Empty;
                    if (entPipeline.Attributes.Contains("pra_firstname"))
                    {
                        firstName = entPipeline.Attributes["pra_firstname"].ToString();
                    } 
                    else if (entPipelineImage.Attributes.Contains("pra_firstname"))
                    {
                        firstName = entPipelineImage.Attributes["pra_firstname"].ToString();
                    }

                    String lastName = String.Empty;
                    if (entPipeline.Attributes.Contains("pra_lastname"))
                    {
                        lastName = entPipeline.Attributes["pra_lastname"].ToString();
                    } 
                    else if (entPipelineImage.Attributes.Contains("pra_lastname"))
                    {
                        lastName = entPipelineImage.Attributes["pra_lastname"].ToString();
                    }

                    String fullName = lastName;
                    if (firstName != String.Empty)
                    {
                        fullName = firstName + " " + lastName;
                    }

                    entPipeline.Attributes.Add("pra_fullname", fullName);

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
