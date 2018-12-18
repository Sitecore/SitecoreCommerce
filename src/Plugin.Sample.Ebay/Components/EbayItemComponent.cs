namespace Plugin.Sample.Ebay.Components
{
    using System.Collections.Generic;

    using global::Plugin.Sample.Ebay.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Pricing;

    public class EbayItemComponent : Component
    {
        public EbayItemComponent()
        {
            this.EbayId = string.Empty;
            this.Fees = new List<AwardedAdjustment>();
            this.Status = string.Empty;
            this.ReasonEnded = string.Empty;
            this.History = new List<HistoryEntryModel>();
        }

        public string EbayId { get; set; }

        public string Status { get; set; }

        public string ReasonEnded { get; set; }
        
        public List<HistoryEntryModel> History { get; set; }

        public List<AwardedAdjustment> Fees { get; set; }
    }
}