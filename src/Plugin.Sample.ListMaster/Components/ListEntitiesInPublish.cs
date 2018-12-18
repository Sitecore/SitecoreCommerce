
namespace Plugin.Sample.ListMaster.Components
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;

    public class ListEntitiesInPublish : Component
    {
        public ListEntitiesInPublish()
        {
            this.History = new List<string>();
            this.LastRow = 0;
            this.LastPublishStart = DateTimeOffset.MinValue;
            this.LastPublishEnd = DateTimeOffset.MinValue;
            this.LastPublishCount = 0;
            this.PublishCycle = 0;
            this.PublishIteration = 0;
        }
        
        public List<string> History { get; set; }
        
        public void AddHistory(string history)
        {
            this.History.Add(history);
        }

        public int LastRow { get; set; }
        
        public DateTimeOffset LastPublishStart { get; set; }
        
        public DateTimeOffset LastPublishEnd { get; set; }
        
        public int LastPublishCount { get; set; }
        
        public int PublishCycle { get; set; }
        
        public int PublishIteration { get; set; }
    }
}