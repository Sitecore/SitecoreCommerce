namespace Plugin.Sample.Ebay.Components
{
    using System;

    using Sitecore.Commerce.Core;

    public class EbayBusinessUserComponent : Component
    {
        public EbayBusinessUserComponent()
        {
            this.EbayToken = string.Empty;
            this.Status = "Pending";
            this.TokenDate = DateTimeOffset.UtcNow;
        }
        
        public string EbayToken { get; set; }

        public DateTimeOffset TokenDate { get; set; }
        
        public string Status { get; set; }
    }
}