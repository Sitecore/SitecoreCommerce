namespace Plugin.Sample.Ebay.Policies
{
    using Sitecore.Commerce.Core;
    
    public class MarketplaceDisplayPolicy : Policy
    {
        public MarketplaceDisplayPolicy()
        {
            this.DateTimeFormat = "yyyy-MMM-dd hh:mm";
        }

        public string DateTimeFormat { get; set; }
    }
}
