namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("FormEditPolicy")]
    public class FormEditPolicy : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormEditPolicy(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "DevOps-EditPolicy")
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);
            
            if (entityViewArgument.Entity == null)
            {
                var name = entityView.EntityId.Replace("Entity-CommerceEnvironment-", "");

                try
                {
                    entityViewArgument.Entity = await this._commerceCommander.Command<GetEnvironmentCommand>()
                        .Process(this._commerceCommander.GetGlobalContext(context.CommerceContext), name);

                    var selectedPolicy = entityViewArgument.Entity.Policies.FirstOrDefault(p => p.PolicyId == entityView.ItemId);
                    if (selectedPolicy != null)
                    {
                        var props = selectedPolicy.GetType().GetProperties();
                        foreach (var prop in props)
                        {
                            if (prop.Name == "Models" || prop.Name == "PolicyId")
                            {
                                //Do nothing
                            }
                            else
                            {
                                var originalType = prop.PropertyType.FullName;
                                if (originalType == "Sitecore.Commerce.Core.EntityReference")
                                {
                                    context.Logger.LogInformation($"DevOps.DoActionEditPolicy.CantProcessProperty: Name={prop.Name}|OriginalType={originalType}");
                                }
                                else if (originalType.Contains("System.Collections.Generic.List"))
                                {
                                    context.Logger.LogInformation($"DevOps.DoActionEditPolicy.CantProcessProperty: Name={prop.Name}|OriginalType={originalType}");
                                }
                                else
                                {
                                    entityView.Properties.Add(
                                        new ViewProperty
                                        {
                                            Name = prop.Name,
                                            IsHidden = false,
                                            IsRequired = false,
                                            RawValue = prop.GetValue(selectedPolicy, null)
                                        });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"Content.SynchronizeContentPath.PathNotFound: Message={ex.Message}");
                }
            }

            return entityView;
        }
    }
}
