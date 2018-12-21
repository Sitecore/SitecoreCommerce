namespace Plugin.Sample.Messaging.Components
{
    using System.Collections.Generic;

    using global::Plugin.Sample.Messaging.Models;

    using Sitecore.Commerce.Core;

    public class HistoryComponent : Component
    {
        public HistoryComponent()
        {
            this.Status = string.Empty;
            this.History = new List<HistoryEntryModel>();
        }
        
        public string Status { get; set; }
        
        public List<HistoryEntryModel> History { get; set; }
    }
}