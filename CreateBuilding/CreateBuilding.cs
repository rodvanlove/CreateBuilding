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

        private ITracingService _tracingService;

        public void Execute(IServiceProvider serviceProvider)
        {
            _tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName != "lead")
                {
                    _tracingService.Trace("Not a Lead");
                    return;
                }
                
                // Get the lead
                //EntityReference leadid = (EntityReference)context.InputParameters["LeadId"];
                Entity lead = service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(new string[] { "new_shortlist" }));

                if (lead.Contains("new_shortlist"))
                {
                    _tracingService.Trace("Target Lead = {0}, Shortlist = {1}", lead.Id.ToString(), lead.Attributes["new_shortlist"].ToString());
                }
                else
                {
                    _tracingService.Trace("No Shortlist");
                    return;
                }

                try
                {
                    string s = lead.Attributes["new_shortlist"].ToString();

                    Guid[] Entities = Array.ConvertAll(s.Split(','), Guid.Parse); ;

                    int x = Entities.Length; // number of entities
                    for (int i = 0; i < x; i++)
                    {
                        Guid id = Entities[i];

                        if (lead != null)
                        {
                            AssociateRequest leadRel = new AssociateRequest
                            {
                                Target = new EntityReference("lead", lead.Id),
                                RelatedEntities = new EntityReferenceCollection
                                    {
                                    new EntityReference("new_building", id)
                                    },
                                Relationship = new Relationship("new_lead_new_building")
                            };

                            // Execute the request.
                            service.Execute(leadRel);
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
}