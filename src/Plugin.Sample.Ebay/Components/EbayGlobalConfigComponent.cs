namespace Plugin.Sample.Ebay.Components
{
    using Sitecore.Commerce.Core;

    public class EbayGlobalConfigComponent : Component
    {
        public EbayGlobalConfigComponent()
        {
            this.ReturnsPolicy = "ReturnsAccepted";
            this.InventorySet = string.Empty;
        }
        
        public string ReturnsPolicy { get; set; }
        
        public string InventorySet { get; set; }
    }
}