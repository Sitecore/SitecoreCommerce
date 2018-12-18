namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionCleanCurrentEnvironment")]
    public class DoActionCleanCurrentEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionCleanCurrentEnvironment(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-CleanCurrentEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }
            
            await this._commerceCommander.Command<CleanEnvironmentCommand>()
                .Process(context.CommerceContext.Environment.Name, context.CommerceContext);
            
            return entityView;
        }
    }
}
