
namespace Plugin.Sample.JsonCommander.Components
{
    using System.Collections.Generic;

    using Plugin.Sample.JsonCommander.Models;

    using Sitecore.Commerce.Core;

    public class ActionHistoryComponent : Component
    {
        public ActionHistoryComponent()
        {
            this.History = new List<ActionHistoryModel>();
        }
        
        public List<ActionHistoryModel> History { get; set; }
        
        public void AddHistory(ActionHistoryModel history)
        {
            this.History.Add(history);
        }
    }
}