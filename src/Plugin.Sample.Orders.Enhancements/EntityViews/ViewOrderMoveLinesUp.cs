namespace Plugin.Sample.Orders.Enhancements.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("ViewOrderMoveLinesUp")]
    public class ViewOrderMoveLinesUp : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null)
            {
                return Task.FromResult(entityView);
            }
            
            if (!entityView.EntityId.Contains("Entity-Order-"))
            {
                return Task.FromResult(entityView);
            }

            if (entityView.Name != "Master")
            {
                return Task.FromResult(entityView);
            }
            
            while (entityView.Properties.Count > 0)
            {
                entityView.Properties.Remove(entityView.Properties.First());
            }

            var linesView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Lines");
            if (linesView != null)
            {
                (linesView as EntityView).DisplayRank = 100;
            }

            var adjustmentsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Adjustments");
            if (adjustmentsView != null)
            {
                (adjustmentsView as EntityView).DisplayRank = 150;
            }
            return Task.FromResult(entityView);
        }
    }
}
