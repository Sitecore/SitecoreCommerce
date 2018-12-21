
namespace Plugin.Sample.Pricing.Generator.Components
{
    using System.Collections.Generic;

    using Plugin.Sample.Pricing.Generator.Models;

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