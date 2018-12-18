
namespace Plugin.Sample.Plugin.Enhancements.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.Plugin.Enhancements.Policies.PluginPolicy;
    
    [PipelineDisplayName("EnsureNavigationView")]
    public class EnsureNavigationView : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "ToolsNavigation")
            {
                return Task.FromResult(entityView);
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            var newEntityView = new EntityView
            {
                Name = "Plugins",
                DisplayName = "Plugins",
                Icon = pluginPolicy.Icon,
                ItemId = "Plugins"
            };

            entityView.ChildViews.Add(newEntityView);

            return Task.FromResult(entityView);
        }
    }
}
