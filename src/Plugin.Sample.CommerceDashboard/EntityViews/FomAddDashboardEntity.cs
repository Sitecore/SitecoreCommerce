
namespace Plugin.Sample.CommerceDashboard.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("FomAddDashboardEntity")]
    public class FomAddDashboardEntity : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "MyDashboard-FormAddDashboardEntity")
            {
                return Task.FromResult(entityView);
            }
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Name",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DisplayName",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty,
                    UiType = "RichText"
                });

            return Task.FromResult(entityView);
        }
    }
}
