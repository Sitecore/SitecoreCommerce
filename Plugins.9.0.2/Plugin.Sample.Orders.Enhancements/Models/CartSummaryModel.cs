// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartSummaryModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>----------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Orders.Enhancements
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Conditions;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the CartSummaryModel Model 
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Model" />
    public class CartSummaryModel : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartSummaryModel"/> class.
        /// </summary>
        public CartSummaryModel()
        {
            CartId = "";
            Status = "";

        }

        /// <summary>
        /// Id of the Cart
        /// </summary>
        public string CartId { get; set; }

        /// <summary>
        /// Status of the Cart
        /// </summary>
        public string Status { get; set; }


    }
}
