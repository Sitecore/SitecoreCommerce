namespace Plugin.Sample.Ebay.Pipelines
{
    using eBay.Service.Core.Soap;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;

    public class PrepareEbayItemPipeline : CommercePipeline<SellableItem, ItemType>, IPrepareEbayItemPipeline
    {
        public PrepareEbayItemPipeline(IPipelineConfiguration<IPrepareEbayItemPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

