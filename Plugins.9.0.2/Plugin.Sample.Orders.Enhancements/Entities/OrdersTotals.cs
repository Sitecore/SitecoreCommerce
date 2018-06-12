// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleDashboardEntity.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   SampleEntity model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Orders.Enhancements
{
    using System;
    using System.Collections.Generic;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;

    /// <summary>
    /// SampleDashboardEntity model.
    /// </summary>
    public class OrdersTotals : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersTotals"/> class.
        /// </summary>
        public OrdersTotals()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;

            this.Totals = new Totals();
            LastSkip = 0;
            this.History = new List<Messaging.HistoryEntryModel>();
            OrderCount = 0;
            MonitorCycle = 0;
            Adjustments = new List<TotalsAdjustmentsModel>();
        }

        /// <summary>
        /// Totals Accumulator
        /// </summary>
        public Totals Totals { get; set; }

        /// <summary>
        /// Location of last read
        /// </summary>
        public Int64 LastSkip { get; set; }

        /// <summary>
        /// History List
        /// </summary>
        public List<Messaging.HistoryEntryModel> History { get; set; }

        /// <summary>
        /// Time the last run started
        /// </summary>
        public DateTimeOffset LastRunStarted { get; set; }


        /// <summary>
        /// Time the last run ended
        /// </summary>
        public DateTimeOffset LastRunEnded { get; set; }

        /// <summary>
        /// Count of Orders
        /// </summary>
        public Int32 OrderCount { get; set; }

        /// <summary>
        /// Current Monitor Cycle
        /// </summary>
        public Int32 MonitorCycle { get; set; }

        /// <summary>
        /// Adjustments
        /// </summary>
        public List<TotalsAdjustmentsModel> Adjustments  {get; set;}

    }
}