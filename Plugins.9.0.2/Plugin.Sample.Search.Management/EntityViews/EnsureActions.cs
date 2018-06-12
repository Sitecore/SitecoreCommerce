
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

            

            var searchScopesView = entityView.ChildViews.FirstOrDefault(p => p.Name == "SearchScopes");
            if (searchScopesView != null)
            {
                var actionsPolicy = searchScopesView.GetPolicy<ActionsPolicy>();

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

            return Task.FromResult(entityView);
        }
    }
}
