
namespace Plugin.Sample.Orders.Enhancements.Entities
{
    using System;
    using System.Collections.Generic;

    using global::Plugin.Sample.Messaging.Models;
    using global::Plugin.Sample.Orders.Enhancements.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;

    public class OrdersTotals : CommerceEntity
    {
        public OrdersTotals()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;

            this.Totals = new Totals();
            this.LastSkip = 0;
            this.History = new List<HistoryEntryModel>();
            this.OrderCount = 0;
            this.MonitorCycle = 0;
            this.Adjustments = new List<TotalsAdjustmentsModel>();
        }
        
        public Totals Totals { get; set; }

        public int LastSkip { get; set; }

        public List<HistoryEntryModel> History { get; set; }
        
        public DateTimeOffset LastRunStarted { get; set; }
        
        public DateTimeOffset LastRunEnded { get; set; }
        
        public int OrderCount { get; set; }
        
        public int MonitorCycle { get; set; }
        
        public List<TotalsAdjustmentsModel> Adjustments {get; set;}
    }
}