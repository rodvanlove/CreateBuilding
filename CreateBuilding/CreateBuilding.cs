using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Microsoft.Xrm.Sdk;
//using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
//using Microsoft.Xrm.Sdk.Discovery;
//using Microsoft.Crm.Sdk.Messages;
//using System.ServiceModel;
//using System.Net;
//using System.ServiceModel.Description;

namespace N_N_Relationship
{
    public class CreateBuilding : IPlugin
    {
        IOrganizationService service;

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            service = serviceFactory.CreateOrganizationService(context.UserId);

            // Get the qualified lead
            EntityReference leadid = (EntityReference)context.InputParameters["LeadId"];
            Entity lead = service.Retrieve(leadid.LogicalName, leadid.Id, new ColumnSet(true));
            try
            {

                // Get the newly created account, contact, opportunity
                //Entity contact = null;
                Entity opportunity = null;
                //Entity account = null;
                foreach (EntityReference created in (IEnumerable<object>)context.OutputParameters["CreatedEntities"])
                {
                    switch (created.LogicalName)
                    {
                        //case "contact":
                        //    contact = service.Retrieve("contact", created.Id, new ColumnSet(true));
                        //    break;
                        //case "account":
                        //    account = service.Retrieve("account", created.Id, new ColumnSet(true));
                        //    break;
                        case "opportunity":
                            opportunity = service.Retrieve("opportunity", created.Id, new ColumnSet(true));
                            break;
                    }
                }

                //create the Query
                QueryExpression query = new QueryExpression();
                query.EntityName = "new_building";
                query.ColumnSet = new ColumnSet(true);
                Relationship relationship = new Relationship();
                query.Criteria = new FilterExpression();
                query.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));
                relationship.SchemaName = "new_lead_new_building";
                RelationshipQueryCollection relatedEntity = new RelationshipQueryCollection();
                relatedEntity.Add(relationship, query);


                //create the request
                RetrieveRequest request = new RetrieveRequest();
                request.RelatedEntitiesQuery = relatedEntity;
                request.ColumnSet = new ColumnSet("leadid");
                request.Target = new EntityReference { Id = leadid.Id, LogicalName = "lead" };

                //execute the request
                RetrieveResponse response = (RetrieveResponse)service.Execute(request);

                // here you can check collection count
                if (((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities)))).Contains(new Relationship("new_lead_new_building")) && ((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("new_lead_new_building")].Entities.Count > 0)
                {
                    int x = ((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("new_lead_new_building")].Entities.Count;
                    for (int i = 0; i < x; i++)
                    {
                        Guid id = ((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("new_lead_new_building")].Entities[i].Id;


                        /*if (contact != null)
                        {
                            AssociateRequest contactRel = new AssociateRequest
                            {
                                Target = new EntityReference("contact", contact.Id),
                                RelatedEntities = new EntityReferenceCollection
                                 {
                                 new EntityReference("new_building", id)
                                 },
                                Relationship = new Relationship("new_contact_bank")
                            };

                            // Execute the request.
                            service.Execute(contactRel);
                        }*/

                        /*if (account != null)
                        {
                            AssociateRequest accountRel = new AssociateRequest
                            {
                                Target = new EntityReference("account", account.Id),
                                RelatedEntities = new EntityReferenceCollection
                                 {
                                 new EntityReference("new_building", id)
                                 },
                                Relationship = new Relationship("new_account_new_building")
                            };

                            // Execute the request.
                            service.Execute(accountRel);
                        }*/

                        if (opportunity != null)
                        {
                            AssociateRequest opportunityRel = new AssociateRequest
                            {
                                Target = new EntityReference("opportunity", opportunity.Id),
                                RelatedEntities = new EntityReferenceCollection
                                 {
                                 new EntityReference("new_building", id)
                                 },
                                Relationship = new Relationship("new_opportunity_new_building")
                            };

                            // Execute the request.
                            service.Execute(opportunityRel);
                        }

                    }
                }
            }

            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An error occured in the Plugin." + ex, ex);
            }

        }
    }
}