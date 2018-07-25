
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.VatTax
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
    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureActions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureActions(CommerceCommander commerceCommander)
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

            if (entityView.Name != "VatTaxDashboard")
            {
                return Task.FromResult(entityView);
            }

            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var tableViewActionsPolicy = entityView.GetPolicy<ActionsPolicy>();
            //This action adds a SampleDashboardEntity
            tableViewActionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "VatTaxDashboard-AddDashboardEntity",
                DisplayName = "Adds a new Vat Tax Entry",
                Description = "Adds a new Vat Tax Entry",
                IsEnabled = true,
                RequiresConfirmation = false,
                EntityView = "VatTaxDashboard-FormAddDashboardEntity",
                Icon = "add"
            });
            //This action removes a SampleDashboardEntity
            tableViewActionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "VatTaxDashboard-RemoveDashboardEntity",
                DisplayName = "Remove Vat Tax Entry",
                Description = "Removes a Vat Tax Entry",
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = "",
                Icon = "delete"
            });

            return Task.FromResult(entityView);
        }
    }
}
