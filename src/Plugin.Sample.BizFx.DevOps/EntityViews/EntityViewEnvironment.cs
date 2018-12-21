namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Caching;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = Plugin.Sample.BizFx.DevOps.Policies.PluginPolicy;

    [PipelineDisplayName("EntityViewEnvironment")]
    public class EntityViewEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public EntityViewEnvironment(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");;

            if (entityView.EntityId == null || !entityView.EntityId.Contains("Entity-CommerceEnvironment-"))
            {
                return entityView;
            }

            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;

            var name = entityView.EntityId.Replace("Entity-CommerceEnvironment-", "");

            try
            {
                var env = await this._commerceCommander.Command<GetEnvironmentCommand>()
                    .Process(this._commerceCommander.GetGlobalContext(context.CommerceContext), name);
                entityViewArgument.Entity = env;

                this.AddEnvironmentView(entityView, context.CommerceContext, env);
                this.AddComponentsListView(entityView, context.CommerceContext, env);
                this.AddPoliciesListView(entityView, context.CommerceContext, env);
            }
            catch(Exception ex)
            {
                context.Logger.LogError($"Content.SynchronizeContentPath.PathNotFound: Message={ex.Message}");
            }

            return entityView;
        }
        
        private void AddEnvironmentView(EntityView entityView, CommerceContext commerceContext, CommerceEnvironment environment)
        {
            if (environment == null)
            {
                return;
            }

            var entityViewEnvironment = new EntityView
            {
                EntityId = string.Empty,
                ItemId = environment.Id,
                DisplayName = "Environment Display Name",
                Name = "Environment - " + environment.Name
            };

            entityViewEnvironment.Properties
                .Add(new ViewProperty { Name = "Environment", RawValue = environment.Name, UiType = "EntityLink" });
            entityViewEnvironment.Properties
                .Add(new ViewProperty { Name = "Policies", RawValue = environment.Policies.Count });
            entityViewEnvironment.Properties
                .Add(new ViewProperty { Name = "Components", RawValue = environment.Components.Count });
            entityViewEnvironment.Properties
                .Add(new ViewProperty { Name = "ArtifactStoreId", RawValue = environment.ArtifactStoreId.ToString("N") });
            entityViewEnvironment.Properties
                .Add(new ViewProperty { Name = "GlobalEnvironmentName", RawValue = this._commerceCommander.CurrentNodeContext(commerceContext).GlobalEnvironmentName });
            entityView.ChildViews.Add(entityViewEnvironment);
        }

        private void AddComponentsListView(EntityView entityView, CommerceContext commerceContext, CommerceEnvironment environment)
        {
            if (environment == null)
            {
                return;
            }

            var componentsListContainer = new EntityView
            {
                EntityId = string.Empty,
                ItemId = string.Empty,
                DisplayName = "Entity Components",
                Name = "Components",
                UiHint = "Table"
            };
            entityView.ChildViews.Add(componentsListContainer);

            foreach(var component in environment.Components)
            {
                var environmentRowEntityView = new EntityView
                {
                    EntityId = entityView.EntityId,
                    ItemId = component.Id,
                    Name = component.GetType().FullName,
                    DisplayName = component.GetType().FullName,
                    UiHint = "Flat"
                };
                componentsListContainer.ChildViews.Add(environmentRowEntityView);

                environmentRowEntityView.Properties.Add(new ViewProperty { Name = "Name", RawValue = component.GetType().FullName });
            }
        }

        private void AddPoliciesListView(EntityView entityView, CommerceContext commerceContext, CommerceEnvironment environment)
        {
            if (environment == null)
            {
                return;
            }

            var pluginPolicy = commerceContext.GetPolicy<PluginPolicy>();

            var localpolicies = new List<Policy>();

            localpolicies.AddRange(environment.Policies);
            
            var policySetPolicies = localpolicies.OfType<PolicySetPolicy>().ToList();

            try
            {
                var policySetsView = new EntityView
                {
                    EntityId = string.Empty,
                    ItemId = string.Empty,
                    DisplayName = "Policy Sets",
                    Name = "PolicySets",
                    UiHint = "Table"
                };
                entityView.ChildViews.Add(policySetsView);

                foreach (var policySetPolicy in policySetPolicies)
                {
                    localpolicies.Remove(policySetPolicy);

                    policySetsView.ChildViews.Add(
                        new EntityView
                        {
                            ItemId = $"{policySetPolicy.PolicyId}",
                            Icon = pluginPolicy.Icon,
                            Properties = new List<ViewProperty> {
                                new ViewProperty {Name = "ItemId", RawValue = $"{policySetPolicy.PolicyId}", UiType = "EntityLink", IsHidden = true },
                                new ViewProperty {Name = "PolicySetId", RawValue = policySetPolicy.PolicySetId }
                            }
                        });
                }
            }
            catch(Exception ex)
            {
                commerceContext.Logger.LogError($"Catalog.DoActionAddList.Exception: Message={ex.Message}");
            }

            var validationPolicies = localpolicies.OfType<ValidationPolicy>().ToList();

            try
            {
                var validationsView = new EntityView
                {
                    EntityId = string.Empty,
                    ItemId = string.Empty,
                    DisplayName = "Validation Policies",
                    Name = "ValidationPolicies",
                    UiHint = "Table"
                };
                entityView.ChildViews.Add(validationsView);

                foreach (var policy in validationPolicies)
                {
                    localpolicies.Remove(policy);

                    var validationAttributes = policy.Models.OfType<ValidationAttributes>().FirstOrDefault(); ;
                    
                    validationsView.ChildViews.Add(
                        new EntityView
                        {
                            ItemId = $"{policy.PolicyId}",
                            Icon = pluginPolicy.Icon,
                            Properties = new List<ViewProperty> {
                                new ViewProperty {Name = "ItemId", RawValue = $"{policy.PolicyId}", UiType = "EntityLink", IsHidden = true },
                                new ViewProperty {Name = "Name", RawValue = policy.TypeFullName },
                                new ViewProperty {Name = "Validator", RawValue = validationAttributes.RegexValidator },
                                new ViewProperty {Name = "Error", RawValue = validationAttributes.RegexValidatorErrorCode },
                                new ViewProperty {Name = "Required", RawValue = validationAttributes.IsRequired },
                                new ViewProperty {Name = "Required", RawValue = validationAttributes.MinLength },
                                new ViewProperty {Name = "Required", RawValue = validationAttributes.MaxLength }
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                commerceContext.Logger.LogError($"DevOps.AddPoliciesListView.Exception: Message={ex.Message}");
            }
            
            var memoryCachePolicies = new List<EntityMemoryCachingPolicy>();
            memoryCachePolicies.AddRange(localpolicies.OfType<EntityMemoryCachingPolicy>());

            var memoryCacheListContainer = new EntityView
            {
                EntityId = string.Empty,
                ItemId = string.Empty,
                DisplayName = "Entity memory Caching",
                Name = "Entity memory Caching",
                UiHint = "Table"
            };
            entityView.ChildViews.Add(memoryCacheListContainer);

            foreach (var memoryCachePolicy in memoryCachePolicies)
            {
                var environmentRowEntityView = new EntityView
                {
                    EntityId = entityView.EntityId,
                    ItemId = memoryCachePolicy.PolicyId,
                    Name = memoryCachePolicy.GetType().Name,
                    DisplayName = memoryCachePolicy.GetType().FullName,
                    UiHint = "Flat"
                };
                memoryCacheListContainer.ChildViews.Add(environmentRowEntityView);

                environmentRowEntityView.Properties.Add(new ViewProperty { Name = "Name", RawValue = memoryCachePolicy.EntityFullName });
                environmentRowEntityView.Properties.Add(new ViewProperty { Name = "AllowCaching", RawValue = memoryCachePolicy.AllowCaching });

                if (memoryCachePolicy.AllowCaching)
                {
                    environmentRowEntityView.Properties.Add(new ViewProperty { Name = "CacheAsEntity", RawValue = memoryCachePolicy.CacheAsEntity });
                    environmentRowEntityView.Properties.Add(new ViewProperty { Name = "CacheName", RawValue = memoryCachePolicy.CacheName });
                    environmentRowEntityView.Properties.Add(new ViewProperty { Name = "Expiration", RawValue = memoryCachePolicy.Expiration });
                    environmentRowEntityView.Properties.Add(new ViewProperty { Name = "HasNegativeCaching", RawValue = memoryCachePolicy.HasNegativeCaching });
                    environmentRowEntityView.Properties.Add(new ViewProperty { Name = "Priority", RawValue = memoryCachePolicy.Priority });
                }
                localpolicies.Remove(memoryCachePolicy);
            }
            
            foreach (var policy in localpolicies)
            {
                var policyName = policy.GetType().Name;

                var policyEntityView = new EntityView
                {
                    EntityId = entityView.EntityId,
                    ItemId = policy.PolicyId,
                    Name = policyName,
                    DisplayName = policyName,
                    UiHint = "Flat"
                };
                entityView.ChildViews.Add(policyEntityView);

                if (policyName == "GlobalEnvironmentPolicy")
                {
                    policyEntityView.DisplayRank = 100;
                }

                if (policyName == "SitecoreConnectionPolicy")
                {
                    policyEntityView.DisplayRank = 0;
                }

                if (policyName == "CsCatalogPolicy")
                {
                    policyEntityView.DisplayRank = 0;
                }

                var props = policy.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (prop.Name == "Models" || prop.Name == "PolicyId")
                    {
                        //DoNothing
                    }
                    else
                    {
                        var newProp = new ViewProperty { Name = prop.Name, RawValue = prop.GetValue(policy, null) };

                        var originalType = prop.PropertyType.FullName;
                        
                        try
                        {
                            if (policyName == "CsCatalogPolicy")
                            {
                                if (newProp.Name == "SchemaTimeout" || newProp.Name == "ItemInformationCacheTimeout" || newProp.Name == "ItemHierarchyCacheTimeout" || newProp.Name == "ItemRelationshipsCacheTimeout" || newProp.Name == "ItemAssociationsCacheTimeout" || newProp.Name == "CatalogCollectionCacheTimeout" || newProp.Name == "SupportedAuthorizationMethods")
                                {
                                    newProp.IsHidden = true;
                                }
                            }

                            if (policyName == "SitecoreConnectionPolicy" && prop.Name == "Host")
                            {
                                newProp.RawValue = $"<a href='http://{newProp.RawValue}' target='_blank' >(Shop){newProp.RawValue}</a><br><a href='http://{newProp.RawValue}/sitecore' target='_blank' >(Admin){newProp.RawValue}/sitecore</a>";
                                newProp.UiType = "Html";
                                newProp.OriginalType = "Html";
                            }
                            
                            if (originalType == "Sitecore.Commerce.Core.EntityReference")
                            {
                                commerceContext.Logger.LogInformation($"DevOps.EntityViewEnvironment.CantProcessProperty: Name={prop.Name}|OriginalType={originalType}");
                                if (newProp.RawValue is EntityReference reference)
                                {
                                    newProp.RawValue = $"<a href='entityView/Master/{reference.EntityTarget}' >{reference.EntityTarget}</a>";
                                    newProp.UiType = "Html";
                                    newProp.OriginalType = "Html";
                                }
                                else
                                {
                                    newProp.RawValue = "[Reference (Null)]";
                                }
                            }
                            else if (newProp.RawValue is List<EntityReference> propList)
                            {
                                var finalValue = string.Empty;
                                foreach(var propRow in propList)
                                {
                                    finalValue = finalValue + $"<a href='entityView/Master/{propRow.EntityTarget}' >{propRow.EntityTarget}</a></br>";
                                    newProp.UiType = "Html";
                                    newProp.OriginalType = "Html";
                                }
                                newProp.RawValue = finalValue;
                            }
                            else if (originalType.Contains("System.Collections.Generic.List"))
                            {
                                if (newProp.RawValue is List<string> stringList)
                                {
                                    var finalValue = string.Empty;
                                    foreach(var value in stringList)
                                    {
                                        finalValue = finalValue + $"{value}<br>";
                                    }
                                    newProp.RawValue = finalValue;
                                    newProp.UiType = "Html";
                                    newProp.OriginalType = "Html";
                                }
                                else
                                {
                                    newProp.RawValue = $"(List-{originalType})";
                                }
                            }
                            else if (originalType.Contains("System.Collections.Generic.IList"))
                            {
                                commerceContext.Logger.LogInformation($"DevOps.EntityViewEnvironment.CantProcessProperty: Name={prop.Name}|OriginalType={originalType}");

                                newProp.RawValue = newProp.RawValue is List<string> list ? $"(String-{list.Count})" : $"(List-{originalType})";
                            }

                            if (newProp.RawValue is bool)
                            {
                                newProp.OriginalType = "System.Boolean";
                            }
                        }
                        catch (Exception ex)
                        {
                            commerceContext.Logger.LogError($"DevOps.EntityViewEnvironment.Exception: Message={ex.Message}|Name={prop.Name}|OriginalType={originalType}");
                        }
                        policyEntityView.Properties.Add(newProp);
                    }
                }
            }
        }
    }
}
