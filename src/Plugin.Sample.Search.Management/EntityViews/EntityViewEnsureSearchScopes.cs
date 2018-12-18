namespace Plugin.Sample.Search.Management.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Search;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.Search.Management.Policies.PluginPolicy;

    [PipelineDisplayName("EntityViewEnsureSearchScopes")]
    public class EntityViewEnsureSearchScopes : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "SearchDashboard")
            {
                return Task.FromResult(entityView);
            }

            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return Task.FromResult(entityView);
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            var scopesView = new EntityView
            {
                EntityId = entityView.EntityId,
                ItemId = string.Empty,
                DisplayName = "Search Scopes",
                Name = "SearchScopes",
                UiHint = "Table",
                Icon = pluginPolicy.Icon
            };
            entityView.ChildViews.Add(scopesView);

            var searchScopePolicies = context.CommerceContext.Environment.GetPolicies<SearchScopePolicy>();

            foreach (var searchScopePolicy in searchScopePolicies)
            {
                var newEntityView = new EntityView
                {
                    Name = string.Empty,
                    DisplayName = string.Empty,
                    Icon = pluginPolicy.Icon,
                    ItemId = searchScopePolicy.Name
                };
                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "Name",
                        IsHidden = false,
                        RawValue = searchScopePolicy.Name
                    });
                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "DeletedListName",
                        IsHidden = false,
                        RawValue = searchScopePolicy.DeletedListName
                    });

                var entityTypes = "<table>";

                foreach(var entityType in searchScopePolicy.EntityTypeNames)
                {
                    entityTypes = entityTypes + $"<tr><td style='color: red;width:100%'>{entityType}</td></tr>";
                }

                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "EntityTypes",
                        IsHidden = false,
                        IsReadOnly = true,
                        OriginalType = "Html",
                        UiType = "Html",
                        RawValue = entityTypes + "</table>"
                    });

                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "FullListName",
                        IsHidden = false,
                        RawValue = searchScopePolicy.FullListName
                    });
                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "IncrementalListName",
                        IsHidden = false,
                        RawValue = searchScopePolicy.IncrementalListName
                    });

                scopesView.ChildViews.Add(newEntityView);
            }

            return Task.FromResult(entityView);
        }
    }
}
