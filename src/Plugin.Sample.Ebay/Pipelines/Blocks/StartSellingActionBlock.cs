namespace Plugin.Sample.Ebay.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using eBay.Service.Core.Soap;
    
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("Ebay.StartSellingActionBlock")]
    public class StartSellingActionBlock : PipelineBlock<ItemType, ItemType, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public StartSellingActionBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<ItemType> Run(ItemType ebayItem, CommercePipelineExecutionContext context)
        {
            Condition.Requires(ebayItem).IsNotNull($"{this.Name}: The argument can not be null");

            try
            {
                var entityView = context.CommerceContext.GetObjects<EntityView>().FirstOrDefault();
                if (entityView != null && entityView.Action == "Ebay-StartSelling")
                {
                    var listingDuration = entityView.Properties.First(p => p.Name == "ListingDuration").Value ?? "";
                    var quantitySubmitted = entityView.Properties.First(p => p.Name == "Quantity").Value ?? "";
                    var quantity = System.Convert.ToInt32(quantitySubmitted);

                    ebayItem.ListingDuration = "Days_" + listingDuration;
                    ebayItem.Quantity = quantity;
                    ebayItem.QuantityAvailable = quantity;
                }
            }
            catch(Exception ex)
            {
                context.Logger.LogError($"Ebay.StartSellingActionBlock.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "StartSellingActionBlock.Run.Exception", new object[] { ex }, ex.Message);
            }
            return ebayItem;
        }
    }
}
