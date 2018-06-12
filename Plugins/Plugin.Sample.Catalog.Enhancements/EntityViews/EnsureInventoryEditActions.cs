
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.Catalog.Enhancements
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
    [PipelineDisplayName("EnsureInventoryEditActions")]
    public class EnsureInventoryEditActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureInventoryEditActions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureInventoryEditActions(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView != null)
            {
                

                if (entityView.EntityId.Contains("Entity-SellableItem-") && string.IsNullOrEmpty(entityView.Action))
                {
                    var pluginPolicy = context.GetPolicy<PluginPolicy>();

                    var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

                    //This action adds a SampleDashboardEntity
                    actionsPolicy.Actions.Add(new EntityActionView
                    {
                        Name = "EditSellableItemInventory",
                        DisplayName = "Edit Inventory",
                        Description = "Adds a new Sample Dashboard Entity",
                        IsEnabled = true,
                        RequiresConfirmation = false, ConfirmationMessage = "This is a confirmation message",
                        EntityView = "EditInventory",
                        Icon = "add"
                    });

                }
            }

            return Task.FromResult(entityView);
        }
    }
}
