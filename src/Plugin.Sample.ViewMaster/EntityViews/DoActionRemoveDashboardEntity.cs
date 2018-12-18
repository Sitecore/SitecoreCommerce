namespace Plugin.Sample.ViewMaster.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionRemoveDashboardEntity")]
    public class DoActionRemoveDashboardEntity : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionRemoveDashboardEntity(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("MyDashboard-RemoveDashboardEntity", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                await this._commerceCommander.Command<DeleteEntityCommand>().Process(context.CommerceContext, entityView.ItemId);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionRemoveDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
