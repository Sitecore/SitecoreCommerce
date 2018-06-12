
namespace Plugin.Sample.ListMaster
{
    using Sitecore.Commerce.Core;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The SampleComponent.
    /// </summary>
    public class ListEntitiesInPublish : Component
    {
    
        /// <summary>
        /// ActionHistoryComponent
        /// </summary>
        public ListEntitiesInPublish()
        {
            History = new List<string>();
            LastRow = 0;
            LastPublishStart = DateTimeOffset.MinValue;
            LastPublishEnd = DateTimeOffset.MinValue;
            LastPublishCount = 0;
            PublishCycle = 0;
            PublishIteration = 0;
        }

        /// <summary>
        /// History
        /// </summary>
        public List<string> History { get; set; }

        /// <summary>
        /// AddHistory
        /// </summary>
        /// <param name="history"></param>
        public void AddHistory(string history)
        {
            History.Add(history);
        }

        /// <summary>
        /// Last Row published
        /// </summary>
        public int LastRow { get; set; }

        /// <summary>
        /// Start of last Publish Run
        /// </summary>
        public DateTimeOffset LastPublishStart { get; set; }

        /// <summary>
        /// End of Last Published Run
        /// </summary>
        public DateTimeOffset LastPublishEnd { get; set; }

        /// <summary>
        /// Artifacts published in last publish
        /// </summary>
        public Int32 LastPublishCount { get; set; }

        /// <summary>
        /// Current Cycle for Publishing
        /// </summary>
        public Int32 PublishCycle { get; set; }

        /// <summary>
        /// The Iteration number of this publish cycle
        /// </summary>
        public Int32 PublishIteration { get; set; }
    }
}