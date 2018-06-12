// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionPullEnvironment.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Plugin.Sample.JsonCommander;

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionPullEnvironment")]
    public class DoActionPullEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        private static readonly JsonSerializerSettings Serializer = new JsonSerializerSettings
        {  
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionEditPolicy"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionPullEnvironment(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="entityView">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (entityView == null
                || !entityView.Action.Equals("DevOps-PullEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var pluginPolicy = context.GetPolicy<PluginPolicy>();

                var environment = entityView.Properties.FirstOrDefault(p => p.Name == "Environment")?.Value;
                var nameAs = entityView.Properties.FirstOrDefault(p => p.Name == "NameAs")?.Value;

                var appService = await this._commerceCommander.GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

                var serviceUri = $"http://{appService.Host}/commerceops/ExportEnvironment(environmentName='{environment}')";
                var jsonResponse = await this._commerceCommander.Command<JsonCommander>().Process(context.CommerceContext, serviceUri);

                dynamic dynJson = JsonConvert.DeserializeObject(jsonResponse.Json);

                string exportedEnvironment = dynJson.value.ToString();

                JObject rss = JObject.Parse(exportedEnvironment);

                JArray policies = (JArray)rss["Policies"]["$values"];

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

                var persistResult = await this._commerceCommander.PersistGlobalEntity(context.CommerceContext,environmentAsEntity);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"DevOps.DoActionPullEnvironment.Exception: Message={ex.Message}");
            }

            return entityView;
        }
   }
}
