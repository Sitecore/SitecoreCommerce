namespace Plugin.Sample.Ebay.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Components;
    using global::Plugin.Sample.Ebay.Entities;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionConfigure")]
    public class DoActionConfigure : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionConfigure(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Ebay-Configure", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var returnsPolicy = entityView.Properties.First(p => p.Name == "ReturnsPolicy").Value ?? "";
                var inventorySet = entityView.Properties.First(p => p.Name == "InventorySet").Value ?? "";
                
                var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);
                if (!ebayConfig.IsPersisted)
                {
                    ebayConfig.Id = "Entity-EbayConfigEntity-Global";
                }
                
                var ebayConfigComponent = ebayConfig.GetComponent<EbayGlobalConfigComponent>();
                ebayConfigComponent.ReturnsPolicy = returnsPolicy;
                ebayConfigComponent.InventorySet = inventorySet;
                
                await this._commerceCommander.PersistEntity(context.CommerceContext, ebayConfig);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionConfigure.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DoActionConfigure.Run.Exception", new object[] { ex }, ex.Message);
            }

            return entityView;
        }
    }
}
