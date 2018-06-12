// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckDeserializedEntityBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the registered plugin block.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Newtonsoft.Json.Linq;
    using System;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    ///  Defines the registered plugin block.
    /// </summary>
    [PipelineDisplayName("CheckDeserializedEntityBlock")]
    public class CheckDeserializedEntityBlock : PipelineBlock<CommerceEntity, CommerceEntity, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        private static readonly JsonSerializerSettings Serializer = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };


        /// <summary>
        /// Initializes a new instance of the <see cref="GetEntityCommandNoDeserialize" /> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public CheckDeserializedEntityBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The list of <see cref="RegisteredPluginModel"/>
        /// </returns>
        public override async Task<CommerceEntity> Run(CommerceEntity arg, CommercePipelineExecutionContext context)
        {
            if (arg != null)
            {
                return arg;
            }

            if (context.CommerceContext.GetMessages().Any(p => p.Code == "Error" && p.CommerceTermKey == "FailedToDeserialize"))
            {
                context.CommerceContext.Logger.LogTrace($"DevOps.CheckDeserializedEntityBlock.Converting");
                //Failure to deserialize
                //Find the entity it was looking for
                var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p=> string.IsNullOrEmpty(p.SerializedEntity) );
                if (foundEntity != null)
                {
                    var entityId = foundEntity.EntityId;
                    context.CommerceContext.Logger.LogTrace($"DevOps.CheckDeserializedEntityBlock.Converting: EntityId={entityId}");

                    var entityAsString = "";

                    entityAsString = await this._commerceCommander.Command<GetEntityCommandNoDeserialize>().Process(context.CommerceContext, entityId);

                    try
                    {
                        if (!string.IsNullOrEmpty(entityAsString))
                        {
                            JObject rss = JObject.Parse(entityAsString);

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
                                catch (Exception ex)
                                {
                                    context.Logger.LogError($"DevOps.DoActionPullEnvironment.Exception: Message={ex.Message}");
                                }
                            }

                            foreach (var kill in killList)
                            {
                                policies.Remove(kill);
                            }

                            var finalJson = rss.ToString();

                            arg = JsonConvert.DeserializeObject<CommerceEnvironment>(finalJson, Serializer);
                            foundEntity.Entity = arg;
                            foundEntity.SerializedEntity = finalJson;
                            context.CommerceContext.GetMessages().RemoveAll(p => p.Code == "Error");
                        }
                        else
                        {
                            context.Logger.LogWarning($"DevOps.CheckDeserializedEntityBlock.EntityStringIsNull: EntityId={entityId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        context.CommerceContext.LogException("DevOps.CheckDeserializedEntityBlock", ex);
                    }
                }
            }

            return arg;
        }
    }
}
