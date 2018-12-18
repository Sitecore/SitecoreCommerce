namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("DoActionEditPolicy")]
    public class DoActionEditPolicy : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionEditPolicy(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-EditPolicy", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var name = entityView.EntityId.Replace("Entity-CommerceEnvironment-", "");

                try
                {
                    var storedEntity = await this._commerceCommander.Command<GetEnvironmentCommand>()
                        .Process(this._commerceCommander.GetGlobalContext(context.CommerceContext), name);

                    var selectedPolicy = storedEntity.Policies.FirstOrDefault(p => p.PolicyId == entityView.ItemId);
                    if (selectedPolicy != null)
                    {
                        foreach (var prop in entityView.Properties)
                        {
                            if (prop.Name == "Models" || prop.Name == "PolicyId")
                            {
                                //Do nothing
                            }
                            else
                            {
                                var policyProperties = selectedPolicy.GetType().GetProperties();

                                var policyProperty = policyProperties.First(p => p.Name == prop.Name);

                                if (prop.OriginalType == "System.Decimal")
                                {
                                    policyProperty.SetValue(selectedPolicy, System.Convert.ToDecimal(prop.Value));
                                }
                                else if (prop.OriginalType == "System.Boolean")
                                {
                                    policyProperty.SetValue(selectedPolicy, System.Convert.ToBoolean(prop.Value));
                                }
                                else if (prop.OriginalType == "Sitecore.Commerce.Core.EntityReference")
                                {
                                    context.Logger.LogInformation($"DevOps.DoActionEditPolicy.CantProcessProperty: Name={prop.Name}|OriginalType={prop.OriginalType}");
                                }
                                else if (prop.OriginalType.Contains("System.Collections.Generic.List"))
                                {
                                    //do nothing, ignore these
                                    context.Logger.LogInformation($"DevOps.DoActionEditPolicy.CantProcessProperty: Name={prop.Name}|OriginalType={prop.OriginalType}");
                                }
                                else
                                {
                                    policyProperty.SetValue(selectedPolicy, prop.Value);
                                }
                            }
                        }
                    }

                    await this._commerceCommander.PersistGlobalEntity(context.CommerceContext, storedEntity);
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"{this.Name}.PathNotFound: Message={ex.Message}");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"{this.Name}.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
