
namespace Plugin.Sample.ContentItemCommander.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = Plugin.Sample.ContentItemCommander.Policies.PluginPolicy;

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
                Name = "MyDashboard",
                DisplayName = "My DashBoard",
                Icon = pluginPolicy.Icon,
                ItemId = "MyDashboard"
            };

            entityView.ChildViews.Add(newEntityView);

            return Task.FromResult(entityView);
        }
    }
}
