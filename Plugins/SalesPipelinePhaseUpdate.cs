﻿using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Collections.ObjectModel;

namespace Plugins
{
    public class SalesPipelinePhaseUpdate : IPlugin
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
                    // Plug-in business logic goes here
                    // Contact Section
                    String firstName = String.Empty;
                    if (pipeline.Attributes.Contains("pra_firstname"))
                    {
                        firstName = pipeline.Attributes["pra_firstname"].ToString();
                    }
                    String lastName = pipeline.Attributes["pra_lastname"].ToString();
                    String phoneNumber = String.Empty;
                    if (pipeline.Attributes.Contains("pra_phonenumber"))
                    {
                        phoneNumber = pipeline.Attributes["pra_phonenumber"].ToString();
                    }
                    String email = pipeline.Attributes["pra_email"].ToString();

                    QueryExpression contactQuery = new QueryExpression("contact");
                    contactQuery.ColumnSet = new ColumnSet(new String[] { "contactid", "emailaddress1" });
                    contactQuery.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, email);

                    EntityCollection contactCollection = service.RetrieveMultiple(contactQuery);

                    if (contactCollection.Entities.Count > 0)
                    {
                        // Update existing contact
                        Guid id = contactCollection.Entities[0].Id;
                        pipeline.Attributes.Add("pra_existingcontact", new EntityReference("contact", id));
                    }
                    else
                    {
                        // Create new contact
                        Entity contactRecord = new Entity("contact");
                        if (firstName != null) { contactRecord.Attributes.Add("firstname", firstName); }
                        contactRecord.Attributes.Add("lastname", lastName);
                        contactRecord.Attributes.Add("emailaddress1", email);
                        if (phoneNumber != null) { contactRecord.Attributes.Add("mobilephone", phoneNumber); }

                        Guid guid = service.Create(contactRecord);
                    }

                    // Account Section
                    String companyName = pipeline.Attributes["pra_companyname"].ToString();
                    String businessPhoneNumber = String.Empty;
                    if (pipeline.Attributes.Contains("pra_businessphonenumber"))
                    {
                        businessPhoneNumber = pipeline.Attributes["pra_phonenumber"].ToString();
                    }

                    QueryExpression accountQuery = new QueryExpression("contact");
                    accountQuery.ColumnSet = new ColumnSet(new String[] { "accountid", "emailaddress1" });
                    accountQuery.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, email);

                    EntityCollection accountCollection = service.RetrieveMultiple(accountQuery);

                    if (accountCollection.Entities.Count > 0)
                    {
                        // Update existing account
                        Guid id = accountCollection.Entities[0].Id;
                        pipeline.Attributes.Add("pra_existingaccount", new EntityReference("account", id));
                    }
                    else
                    {
                        // Create new account
                        Entity accountRecord = new Entity("account");
                        accountRecord.Attributes.Add("name", companyName);
                        if (businessPhoneNumber != null) { accountRecord.Attributes.Add("telephone1", businessPhoneNumber); }

                        Guid guid = service.Create(accountRecord);
                    }

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