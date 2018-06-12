//-----------------------------------------------------------------------
// <copyright file="ShoppingCartTotalRenderingModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>Defines the ConfirmRenderingModel class.</summary>
//-----------------------------------------------------------------------
// Copyright 2017 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing permissions
// and limitations under the License.
// -------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Sitecore.Commerce.Plugin.SxaRepo
{
    //using Foundation.Common.Models;
    //using Mvc.Extensions;
    //using Mvc.Presentation;

    /// <summary>
    /// rendering model for Shopping Cart Total component
    /// </summary>
    public class ShoppingCartTotalRenderingModel : Model
    {
        /// <summary>
        /// Gets or sets the order total header tooltip[ text
        /// </summary>
        public string OrderTotalHeaderTooltip { get; set; }

        /// <summary>
        /// Gets or sets the tolltip for total payment label
        /// </summary>
        public string PaymentTotalTooltip { get; set; }

        /// <summary>
        /// Gets or sets the toltip for VAT label
        /// </summary>
        public string VatTooltip { get; set; }

        /// <summary>
        /// Gets or sets the tooltip for the shipping label
        /// </summary>
        public string ShippingTooltip { get; set; }

        /// <summary>
        /// Gets or sets the order total toolltip
        /// </summary>
        public string OrderTotalTooltip { get; set; }

        /// <summary>
        /// Gets or sets the tooltip for the Savings label
        /// </summary>
        public string SavingsTooltip { get; set; }

        public void Initialize()
        {
            //var currentRendering = RenderingContext.CurrentOrNull.ValueOrDefault(context => context.Rendering);
            //if (currentRendering != null)
            //{
            //    var item = currentRendering.Item;
            //    this.OrderTotalHeaderTooltip = item.Fields["Order Total Header Tooltip"].Value;
            //    this.PaymentTotalTooltip = item.Fields["Subtotal Tooltip"].Value;
            //    this.VatTooltip = item.Fields["VAT Tooltip"].Value;
            //    this.ShippingTooltip = item.Fields["Shipping Tooltip"].Value;
            //    this.OrderTotalTooltip = item.Fields["Order Total Tooltip"].Value;
            //    this.SavingsTooltip = item.Fields["Savings Tooltip"].Value;
            //}
        }
    }
}
