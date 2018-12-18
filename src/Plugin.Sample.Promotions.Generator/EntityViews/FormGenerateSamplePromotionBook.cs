
namespace Plugin.Sample.Promotions.Generator.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("FormGenerateSamplePromotionBook")]
    public class FormGenerateSamplePromotionBook : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormGenerateSamplePromotionBook(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Promotions-GenerateSamplePromotionBook")
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
                    RawValue = string.Empty
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Promotions",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = 10
                });

            return Task.FromResult(entityView);
        }
    }

}
