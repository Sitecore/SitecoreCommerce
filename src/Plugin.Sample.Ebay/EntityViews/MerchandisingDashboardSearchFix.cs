namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("MerchandisingDashboardSearchFix")]
    public class MerchandisingDashboardSearchFix : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "MerchandisingDashboard")
            {
                return Task.FromResult(entityView);
            }

            return Task.FromResult(entityView);
        }
    }
}
