namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Plugin.Sample.BizFx.DevOps.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionRemoveAppService")]
    public class DoActionRemoveAppService : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionRemoveAppService(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-RemoveAppService", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }
            var findResult = await this._commerceCommander.GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

            if (findResult != null)
            {
                await this._commerceCommander.Command<DeleteEntityCommand>().Process(context.CommerceContext, entityView.ItemId);
            }

            return entityView;
        }
    }
}
