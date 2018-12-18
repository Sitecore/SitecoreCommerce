
namespace Plugin.Sample.JsonCommander.Models
{
    using System;

    using Sitecore.Commerce.Core;

    public class ActionHistoryModel : Model
    {
        public ActionHistoryModel()
        {
            this.Id = string.Empty;
            this.Completed = DateTime.UtcNow;
            this.Response = string.Empty;
            this.JSON = string.Empty;
        }

        public string Id { get; private set; }

        public string Response { get; set; }

        public DateTime Completed { get; set; }
        
        public string JSON { get; set; }
        
        public string EntityId { get; set; }
        
        public string ItemId { get; set; }
    }
}
