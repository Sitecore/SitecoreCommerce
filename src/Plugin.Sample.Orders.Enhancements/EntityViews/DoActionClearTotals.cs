namespace Plugin.Sample.Orders.Enhancements.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Orders.Enhancements.Commands;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionClearTotals")]
    public class DoActionClearTotals : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionClearTotals(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Orders.Enhancements.ClearTotals", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                await this._commerceCommander.Command<MonitorTotalsCommand>().ClearTotals(context.CommerceContext);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
