// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderSummaryModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>----------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Orders.Enhancements
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Conditions;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the RunningCommands Model 
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Model" />
    public class OrderSummaryModel : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSummaryModel"/> class.
        /// </summary>
        public OrderSummaryModel()
        {
            OrderId = "";
            ConfirmationId = "";
            Status = "";

        }

        /// <summary>
        /// Order Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Confirmation Id
        /// </summary>
        public string ConfirmationId { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }


    }
}
