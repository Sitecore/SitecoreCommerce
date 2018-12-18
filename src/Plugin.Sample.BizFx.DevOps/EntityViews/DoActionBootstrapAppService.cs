namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionBootstrapAppService")]
    public class DoActionBootstrapAppService : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
    
        public DoActionBootstrapAppService(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-BootStrapAppService", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }
            await this._commerceCommander.Command<BootstrapCommand>().Process(context.CommerceContext);
            
            return entityView;
        }
    }
}
