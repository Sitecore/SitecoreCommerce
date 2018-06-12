
using System.Threading.Tasks;
using System.Linq;

namespace Plugin.Sample.Search.Management
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.Plugin.Search;

    /// <summary>
    /// Defines a block which EntityViewEnsureSearchScopes.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EntityViewEnsureSearchScopes")]
    public class EntityViewEnsureSearchScopes : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityViewEnsureSearchScopes"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EntityViewEnsureSearchScopes(CommerceCommander commerceCommander)
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

            if (entityView.Name != "SearchDashboard")
            {
                return Task.FromResult(entityView);
            }

            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return Task.FromResult(entityView);
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>()
                .CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            var scopesView = new EntityView
            {
                EntityId = entityView.EntityId,
                ItemId = "",
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
                    Name = "",
                    DisplayName = "",
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
