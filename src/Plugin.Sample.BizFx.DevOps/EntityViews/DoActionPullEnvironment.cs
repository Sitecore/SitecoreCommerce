namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Plugin.Sample.BizFx.DevOps.Entities;
    using Plugin.Sample.JsonCommander.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("DoActionPullEnvironment")]
    public class DoActionPullEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        private static readonly JsonSerializerSettings Serializer = new JsonSerializerSettings
        {  
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };

        public DoActionPullEnvironment(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-PullEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            { 
                var environment = entityView.Properties.FirstOrDefault(p => p.Name == "Environment")?.Value;
                var nameAs = entityView.Properties.FirstOrDefault(p => p.Name == "NameAs")?.Value;

                var appService = await this._commerceCommander.GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

                var serviceUri = $"http://{appService.Host}/commerceops/ExportEnvironment(environmentName='{environment}')";
                var jsonResponse = await this._commerceCommander.Command<JsonCommander>().Process(context.CommerceContext, serviceUri);

                dynamic dynJson = JsonConvert.DeserializeObject(jsonResponse.Json);

                string exportedEnvironment = dynJson.value.ToString();

                var rss = JObject.Parse(exportedEnvironment);

                var policies = (JArray)rss["Policies"]["$values"];

                var killList = new List<JToken>();
                
                foreach (var policy in policies)
                {
                    try
                    {
                        var policyType = policy["$type"].ToString();

                        if (policyType == "Sitecore.Commerce.Plugin.Customers.Cs.ProfilesSqlPolicy, Sitecore.Commerce.Plugin.Customers.Cs"
                            || policyType == "Sitecore.Commerce.Plugin.Customers.Cs.SitecoreUserTermsPolicy, Sitecore.Commerce.Plugin.Customers.Cs"
                            || policyType == "Sitecore.Commerce.Plugin.Customers.Cs.ProfilesCsCachePolicy, Sitecore.Commerce.Plugin.Customers.Cs")
                        {
                            killList.Add(policy);
                        }
                    }
                    catch(Exception ex)
                    {
                        context.Logger.LogError($"DevOps.DoActionPullEnvironment.Exception: Message={ex.Message}");
                    }
                }

                foreach(var kill in killList)
                {
                    policies.Remove(kill);
                }

                var finalJson = rss.ToString();

                var environmentAsEntity = JsonConvert.DeserializeObject<CommerceEnvironment>(finalJson, Serializer);

                environmentAsEntity.IsPersisted = false;
                environmentAsEntity.Version = 1;
                environmentAsEntity.Id = $"Entity-CommerceEnvironment-{nameAs}";
                environmentAsEntity.Name = nameAs;
                environmentAsEntity.ArtifactStoreId = Guid.NewGuid();
                environmentAsEntity.GetComponent<ListMembershipsComponent>().Memberships.Add("CommerceEnvironments");

                await this._commerceCommander.PersistGlobalEntity(context.CommerceContext,environmentAsEntity);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"DevOps.DoActionPullEnvironment.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
