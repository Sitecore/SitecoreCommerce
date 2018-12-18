namespace Plugin.Sample.BizFx.DevOps.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Plugin.Sample.BizFx.DevOps.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("CheckDeserializedEntityBlock")]
    public class CheckDeserializedEntityBlock : PipelineBlock<CommerceEntity, CommerceEntity, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        private static readonly JsonSerializerSettings Serializer = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };

        public CheckDeserializedEntityBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<CommerceEntity> Run(CommerceEntity arg, CommercePipelineExecutionContext context)
        {
            if (arg != null)
            {
                return arg;
            }

            if (context.CommerceContext.GetMessages().Any(p => p.Code == "Error" && p.CommerceTermKey == "FailedToDeserialize"))
            {
                context.CommerceContext.Logger.LogTrace($"DevOps.CheckDeserializedEntityBlock.Converting");
                var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => string.IsNullOrEmpty(p.SerializedEntity) );
                if (foundEntity != null)
                {
                    var entityId = foundEntity.EntityId;
                    context.CommerceContext.Logger.LogTrace($"DevOps.CheckDeserializedEntityBlock.Converting: EntityId={entityId}");

                    var entityAsString = string.Empty;

                    entityAsString = await this._commerceCommander.Command<GetEntityCommandNoDeserialize>().Process(context.CommerceContext, entityId);

                    try
                    {
                        var rss = JObject.Parse(entityAsString);
                        var policies = (JArray)rss["Policies"]["$values"];
                        if (!string.IsNullOrEmpty(entityAsString))
                        {
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
