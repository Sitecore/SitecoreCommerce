
namespace Plugin.Sample.Pricing.Generator.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("FormGenerateSamplePriceBook")]
    public class FormGenerateSamplePriceBook : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Pricing-GenerateSamplePriceBook")
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
                    Name = "PriceCards",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = 10
                });

            return Task.FromResult(entityView);
        }
    }
}
