namespace Plugin.Sample.Ebay.Pipelines.Blocks
{
    using System;
    using System.Threading.Tasks;

    using eBay.Service.Core.Soap;
   
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("Ebay.PrepareItemStart")]
    public class PrepareItemStartBlock : PipelineBlock<SellableItem, ItemType, CommercePipelineExecutionContext>
    {
        public override async Task<ItemType> Run(SellableItem sellableItem, CommercePipelineExecutionContext context)
        {
            Condition.Requires(sellableItem).IsNotNull($"{this.Name}: The argument can not be null");

            var item = new ItemType();
            try
            {
                item = new ItemType
                {
                    SKU = sellableItem.Id.Replace("Entity-SellableItem-", ""),
                    Currency = CurrencyCodeType.USD,
                    Country = CountryCodeType.US,
                    ListingDuration = "Days_7",
                    PrimaryCategory = new CategoryType { CategoryID = "20713" },
                    Location = "Dallas, TX",
                    Quantity = 10
                };
            }
            catch(Exception ex)
            {
                context.Logger.LogError($"Ebay.PrepareItemStartBlock.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "PrepareItemStartBlock.Run.Exception", new object[] { ex }, ex.Message);
            }

            return item;
        }
    }
}
