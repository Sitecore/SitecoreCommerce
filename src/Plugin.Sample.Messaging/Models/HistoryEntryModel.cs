namespace Plugin.Sample.Messaging.Models
{
    using System;

    using Sitecore.Commerce.Core;
    
    public class HistoryEntryModel : Model
    {
        public HistoryEntryModel()
        {
            this.Id = Guid.NewGuid().ToString("N");
            this.EventDate = DateTimeOffset.Now;
            this.EventMessage = string.Empty;
            this.EventUser = string.Empty;
        }
        
        public string Id { get; private set; }
        
        public DateTimeOffset EventDate { get; set; }
        
        public string EventMessage { get; set; }
        
        public string EventUser { get; set; }

        public string EventData { get; set; }
    }
}
