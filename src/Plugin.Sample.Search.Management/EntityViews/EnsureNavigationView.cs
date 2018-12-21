
namespace Plugin.Sample.Search.Management.EntityViews
{
    using System.Threading.Tasks;

    using global::Plugin.Sample.Plugin.Enhancements.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.Search.Management.Policies.PluginPolicy;

    [PipelineDisplayName("EnsureNavigationView")]
    public class EnsureNavigationView : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public EnsureNavigationView(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "ToolsNavigation")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();
            
            var userPluginOptions = await this._commerceCommander.Command<PluginCommander>()
                .CurrentUserSettings(context.CommerceContext, this._commerceCommander);
            if (userPluginOptions.EnabledPlugins.Contains("Plugin.Sample.Search.Management"))
            {
                if (userPluginOptions.HasPolicy<PluginPolicy>())
                {
                    pluginPolicy = userPluginOptions.GetPolicy<PluginPolicy>();
                }
                else
                {
                    pluginPolicy.IsDisabled = false;
                }
            }
            else
            {
                pluginPolicy.IsDisabled = true;
            }

            if (!pluginPolicy.IsDisabled)
            {
                var newEntityView = new EntityView
                {
                    Name = "SearchScopes",
                    DisplayName = "Search Management",
                    Icon = pluginPolicy.Icon,
                    ItemId = "SearchDashboard"
                };

                entityView.ChildViews.Add(newEntityView);
            }

            return entityView;
        }
    }
}
