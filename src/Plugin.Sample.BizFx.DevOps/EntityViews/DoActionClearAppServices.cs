namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Plugin.Sample.BizFx.DevOps.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionClearAppServices")]
    public class DoActionClearAppServices : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionClearAppServices(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-ClearAppServices", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }
            var appServices = await this._commerceCommander.Command<ListCommander>().GetListItems<AppService>(context.CommerceContext,"AppServices", 0, 999);

            foreach(var appService in appServices)
            {
                await this._commerceCommander.Command<DeleteEntityCommand>().Process(context.CommerceContext, appService.Id);
            }

            return entityView;
        }
    }
}
