
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.Search.Management
{
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// Defines a block which validates, Actions for this Plugin.
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

            if (entityView.Name != "SearchDashboard" && entityView.Name != "Search-Scopes")
            {
                return Task.FromResult(entityView);
            }

            entityView.Icon = "find_text";

            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            if (entityView.Name == "Search-Scopes")
            {
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Search-RebuildScope",
                    DisplayName = "Rebuild Scope",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    UiHint = "",
                    Icon = "chart_funnel"
                });
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Search-DeleteSearchIndex",
                    DisplayName = "Delete Search Index",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    UiHint = "",
                    Icon = "delete"
                });
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Search-CreateSearchIndex",
                    DisplayName = "Create Search Index",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "Search-CreateSearchIndex",
                    UiHint = "",
                    Icon = "add"
                });

            }

            var flatEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Example Flat View");

            if (flatEntityView != null)
            {
                var flatEntityViewActionsPolicy = flatEntityView.GetPolicy<ActionsPolicy>();

                //This action creates an Action that allows you to show a another EntityView
                flatEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ViewProductLines",
                    DisplayName = "Example Flat View Action",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "MyDashboard-MyFlatView",
                    UiHint = "RelatedList"
                });
            }

            var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Example Table View");

            if (tableEntityView != null)
            {
                var tableEntityViewActionsPolicy = tableEntityView.GetPolicy<ActionsPolicy>();

                //This action creates an Action that allows you to show a another EntityView
                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ViewProductLines",
                    DisplayName = "Example Table View Action",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "MyDashboard-MyTableView",
                    UiHint = "RelatedList"
                });
            }

            return Task.FromResult(entityView);
        }
    }
}
