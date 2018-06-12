//-----------------------------------------------------------------------
// <copyright file="ShippingOptionJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>Defines the ShippingOptionJsonResult class.</summary>
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

namespace Sitecore.Commerce.XA.Feature.Cart.Models.JsonResults
{
    //using Entities.Shipping;
    //using Foundation.Common;
    using Foundation.Common.Models.JsonResults;

    public class ShippingOptionJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingOptionJsonResult"/> class.
        /// </summary>
        /// <param name="storefrontContext">The storefront context.</param>
        //public ShippingOptionJsonResult([NotNull]IStorefrontContext storefrontContext)
        //    : base(storefrontContext)
        //{
        //}

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>
        /// The external identifier.
        /// </value>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the shipping option.
        /// </summary>
        /// <value>
        /// The type of the shipping option.
        /// </value>
        //public Sitecore.Commerce.Entities.Shipping.ShippingOptionType ShippingOptionType { get; set; }

        /// <summary>
        /// Gets or sets the name of the shop.
        /// </summary>
        /// <value>
        /// The name of the shop.
        /// </value>
        public string ShopName { get; set; }

        /// <summary>
        /// Initializes the specified shipping option.
        /// </summary>
        /// <param name="shippingOption">The shipping option.</param>
        //public virtual void Initialize(ShippingOption shippingOption)
        //{
        //    if (shippingOption == null)
        //    {
        //        return;
        //    }

        //    this.ExternalId = shippingOption.ExternalId;
        //    this.Description = shippingOption.Description;
        //    this.Name = this.StorefrontContext.GetShippingName(shippingOption.Name);
        //    this.Description = shippingOption.Description;
        //    this.ShippingOptionType = shippingOption.ShippingOptionType;
        //    this.ShopName = shippingOption.ShopName;
        //}
    }
}