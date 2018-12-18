namespace Plugin.Sample.Search.Management.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
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
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = "chart_funnel"
                });
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Search-DeleteSearchIndex",
                    DisplayName = "Delete Search Index",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = "delete"
                });
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Search-CreateSearchIndex",
                    DisplayName = "Create Search Index",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "Search-CreateSearchIndex",
                    UiHint = string.Empty,
                    Icon = "add"
                });
            }

            return Task.FromResult(entityView);
        }
    }
}
