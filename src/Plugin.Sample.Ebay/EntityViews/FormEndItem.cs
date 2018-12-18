
namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("FormEndItem")]
    public class FormEndItem : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Ebay-FormEndItem")
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
                    Name = "Reason",
                    RawValue = string.Empty,
                    Policies = new List<Policy>
                    {
                        new AvailableSelectionsPolicy
                        {
                            List = new List<Selection>(){
                                new Selection { Name = "NotAvailable", DisplayName = "Not Available", IsDefault = true },
                                new Selection { Name = "Incorrect", DisplayName = "Incorrect Item", IsDefault = false },
                                new Selection { Name = "LostOrBroken", DisplayName = "Item was Lost or Broken", IsDefault = false },
                                new Selection { Name = "OtherListingError", DisplayName = "Item was Listed Incorrectly", IsDefault = false },
                                new Selection { Name = "SellToHighBidder", DisplayName = "Item was Sold to a High Bidder", IsDefault = false }
                            }
                        }
                    },
                    UiType = "Dropdown"
                });

            return Task.FromResult(entityView);
        }
    }

}
