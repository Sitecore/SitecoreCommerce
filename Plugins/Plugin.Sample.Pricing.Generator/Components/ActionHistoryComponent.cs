
namespace Plugin.Sample.Pricing.Generator
{
    using Sitecore.Commerce.Core;
    using System.Collections.Generic;

    /// <summary>
    /// The SampleComponent.
    /// </summary>
    public class ActionHistoryComponent : Component
    {
    
        /// <summary>
        /// ActionHistoryComponent
        /// </summary>
        public ActionHistoryComponent()
        {
            History = new List<ActionHistoryModel>();
        }

        /// <summary>
        /// History
        /// </summary>
        public List<ActionHistoryModel> History { get; set; }

        /// <summary>
        /// AddHistory
        /// </summary>
        /// <param name="history"></param>
        public void AddHistory(ActionHistoryModel history)
        {
            History.Add(history);
        }
    }
}