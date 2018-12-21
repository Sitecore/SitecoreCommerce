namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Threading.Tasks;

    using global::Plugin.Sample.Plugin.Enhancements;
    using global::Plugin.Sample.Plugin.Enhancements.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

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

            var pluginPolicy = context.GetPolicy<global::Plugin.Sample.Ebay.Policies.PluginPolicy>();

            var userPluginOptions = await this._commerceCommander.Command<PluginCommander>()
                .CurrentUserSettings(context.CommerceContext, this._commerceCommander);
            if (userPluginOptions.EnabledPlugins.Contains("Sitecore.Commerce.Plugin.Ebay"))
            {
                if (userPluginOptions.HasPolicy<global::Plugin.Sample.Ebay.Policies.PluginPolicy>())
                {
                    pluginPolicy = userPluginOptions.GetPolicy<global::Plugin.Sample.Ebay.Policies.PluginPolicy>();
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
                    Name = "MarketplacesDashboard",
                    DisplayName = "Marketplaces",
                    Icon = pluginPolicy.Icon,
                    ItemId = "MarketplacesDashboard"
                };

                entityView.ChildViews.Add(newEntityView);
            }

            return entityView;
        }
    }
}
