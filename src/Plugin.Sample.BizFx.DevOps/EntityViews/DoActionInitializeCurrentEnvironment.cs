namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Plugin.Sample.BizFx.DevOps.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("DoActionInitializeCurrentEnvironment")]
    public class DoActionInitializeCurrentEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionInitializeCurrentEnvironment(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-InitializeCurrentEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            await this._commerceCommander.GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

            try
            {
                await this._commerceCommander.Command<InitializeEnvironmentCommand>()
                    .Process(context.CommerceContext, context.CommerceContext.Environment.Name);
            }
            catch (Exception ex)
            {
                context.CommerceContext.LogException("DoActionInitializeCurrentEnvironment", ex);
            }

            return entityView;
        }
    }
}
