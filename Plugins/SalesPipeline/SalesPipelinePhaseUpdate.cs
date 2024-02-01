using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using MyCompany.Practice.Plugins.Helper;

namespace MyCompany.Practice.Plugins.SalesPipeline
{
    public class SalesPipelinePhaseUpdate : IPlugin
    {
        void HandleContactSection(Entity entPipeline, Entity entPipelineImage, IOrganizationService service)
        {
            // Check if pipeline phase updated to "Meeting Requirement"
            if (((OptionSetValue)entPipeline.Attributes["pra_pipelinephase"]).Value == 894870001)
            {
                String firstName = String.Empty;
                if (entPipelineImage.Attributes.Contains("pra_firstname"))
                {
                    firstName = entPipelineImage.Attributes["pra_firstname"].ToString();

                }
                String lastName = entPipelineImage.Attributes["pra_lastname"].ToString();
                String phoneNumber = String.Empty;
                if (entPipelineImage.Attributes.Contains("pra_phonenumber"))
                {
                    phoneNumber = entPipelineImage.Attributes["pra_phonenumber"].ToString();
                }
                String email = entPipelineImage.Attributes["pra_email"].ToString();

                QueryExpression contactQuery = new QueryExpression("contact");
                contactQuery.ColumnSet = new ColumnSet(new String[] { "contactid", "emailaddress1" });
                contactQuery.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, email);

                EntityCollection entColMatchingContacts = service.RetrieveMultiple(contactQuery);

                if (entColMatchingContacts.Entities.Count > 0)
                {
                    // Update existing contact
                    Guid id = entColMatchingContacts.Entities[0].Id;
                    entPipeline.Attributes.Add("pra_existingcontact", new EntityReference("contact", id));
                }
                else
                {
                    // Create new contact
                    Entity entContactRecord = new Entity("contact");
                    if (String.IsNullOrEmpty(firstName)) { entContactRecord.Attributes.Add("firstname", firstName); }
                    entContactRecord.Attributes.Add("lastname", lastName);
                    entContactRecord.Attributes.Add("emailaddress1", email);
                    if (String.IsNullOrEmpty(phoneNumber)) { entContactRecord.Attributes.Add("mobilephone", phoneNumber); }

                    Guid contactId = service.Create(entContactRecord);
                    entPipeline.Attributes.Add("pra_existingcontact", new EntityReference("contact", contactId));
                }
            }
        }
        void HandleAccountSection(Entity entPipeline, Entity entPipelineImage, IOrganizationService service)
        {
            // Check if pipeline phase updated to "Meeting Requirement"
            if (((OptionSetValue)entPipeline.Attributes["pra_pipelinephase"]).Value == 894870001)
            {
                String companyName = entPipelineImage.Attributes["pra_companyname"].ToString();
                String businessPhoneNumber = String.Empty;
                if (entPipelineImage.Attributes.Contains("pra_businessphonenumber"))
                {
                    businessPhoneNumber = entPipelineImage.Attributes["pra_phonenumber"].ToString();
                }

                QueryExpression accountQuery = new QueryExpression("account");
                accountQuery.ColumnSet = new ColumnSet(new String[] { "accountid", "name" });
                accountQuery.Criteria.AddCondition("name", ConditionOperator.Equal, companyName);

                EntityCollection entColMatchingAccounts = service.RetrieveMultiple(accountQuery);

                if (entColMatchingAccounts.Entities.Count > 0)
                {
                    // Update existing account
                    Guid id = entColMatchingAccounts.Entities[0].Id;
                    entPipeline.Attributes.Add("pra_existingaccount", new EntityReference("account", id));
                }
                else
                {
                    // Create new account
                    Entity entAccountRecord = new Entity("account");
                    entAccountRecord.Attributes.Add("name", companyName);
                    if (String.IsNullOrEmpty(businessPhoneNumber)) { entAccountRecord.Attributes.Add("telephone1", businessPhoneNumber); }

                    Guid accountId = service.Create(entAccountRecord);
                    entPipeline.Attributes.Add("pra_existingaccount", new EntityReference("account", accountId));
                }
            }
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            // Getting Services using Helper
            var (tracingService, context, service) = ServiceHelper.GetTracingService(serviceProvider, out var executionContext, out var orgService);
             
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {  
                Entity entPipeline = (Entity)context.InputParameters["Target"];
                Entity entPipelineImage = (context.PreEntityImages.Contains("pipelineImage") && context.PreEntityImages["pipelineImage"] != null)
                    ? (Entity)context.PreEntityImages["pipelineImage"]
                    : throw new InvalidPluginExecutionException("Pre entity image is not registered.");

                try
                {
                    // Contact Section in SalesPipelinePhase
                    HandleContactSection(entPipeline, entPipelineImage, service);

                    // Account Section in SalesPipelinePhase
                    HandleAccountSection(entPipeline, entPipelineImage, service);
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}\n{1}", ex.Message, ex.StackTrace);
                    throw new InvalidPluginExecutionException($"FollowUpPlugin: {ex.Message}");
                }
            }
        }
    }
}
