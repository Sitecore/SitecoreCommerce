
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.Orders.Enhancements
{
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureActionOrders")]
    public class EnsureActionOrders : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewOrders"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureActionOrders(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

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
                EntityView = "",
                Icon = "add"
            });

            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "Orders.Enhancements.ClearTotals",
                DisplayName = "Clear Totals",
                Description = "desc Orders.Enhancements.ClearTotals",
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = "",
                Icon = "add"
            });



            return Task.FromResult(entityView);
        }
    }
}
