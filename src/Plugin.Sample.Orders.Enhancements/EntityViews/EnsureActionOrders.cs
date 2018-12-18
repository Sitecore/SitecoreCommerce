namespace Plugin.Sample.Orders.Enhancements.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("EnsureActionOrders")]
    public class EnsureActionOrders : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.Name != "OrdersDashboard")
            {
                return Task.FromResult(entityView);
            }

            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();
            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "Orders.Enhancements.MonitorTotals",
                DisplayName = "Monitor Totals",
                Description = "desc Orders.Enhancements.MonitorTotals",
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = string.Empty,
                Icon = "add"
            });

            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "Orders.Enhancements.ClearTotals",
                DisplayName = "Clear Totals",
                Description = "desc Orders.Enhancements.ClearTotals",
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = string.Empty,
                Icon = "add"
            });

            return Task.FromResult(entityView);
        }
    }
}
