
namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("FormStartSellingAll")]
    public class FormStartSellingAll : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormStartSellingAll(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Ebay-FormStartSellingAll")
            {
                return Task.FromResult(entityView);
            }
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = string.Empty,
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<img alt='This is the alternate' height=50 width=100 src='https://www.paypalobjects.com/webstatic/en_AR/mktg/merchant/pages/sell-on-ebay/image-ebay.png' style=''/>"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ListingDuration",
                    DisplayName = "Listing Duration (Days)",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = 10
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ImmediateListing",
                    DisplayName = "Publish Immediately to Ebay",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = true
                });

            return Task.FromResult(entityView);
        }
    }
}
