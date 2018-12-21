namespace Plugin.Sample.Orders.Enhancements.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Orders.Enhancements.Commands;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("DoActionMonitorTotals")]
    public class DoActionMonitorTotals : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionMonitorTotals(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Orders.Enhancements.MonitorTotals", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            try
            {
                Task.Factory.StartNew(() => this._commerceCommander.Command<MonitorTotalsCommand>().Process(context.CommerceContext));
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddDashboardEntity.Exception: Message={ex.Message}");
            }

            return Task.FromResult(entityView);
        }
    }
}
