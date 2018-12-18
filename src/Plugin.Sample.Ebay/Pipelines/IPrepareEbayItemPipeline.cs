namespace Plugin.Sample.Ebay.Pipelines
{
    using eBay.Service.Core.Soap;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("Prepare an Ebay Item from a SellableItem for Syncing to Ebay")]
    public interface IPrepareEbayItemPipeline : IPipeline<SellableItem, ItemType, CommercePipelineExecutionContext>
    {
    }
}
