//-----------------------------------------------------------------------
// <copyright file="ShippingInfoJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>Defines the ShippingInfoJsonResult class.</summary>
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
    //using Entities.Carts;
    //using Entities.Shipping;
    using Foundation.Common;
    using Foundation.Common.Models.JsonResults;
    //using Sitecore.Data;
    using System.Collections.Generic;
    using System.Linq;

    public class ShippingInfoJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingInfoJsonResult"/> class.
        /// </summary>
        /// <param name="storefrontContext">The storefront context.</param>
        //public ShippingInfoJsonResult([NotNull]IStorefrontContext storefrontContext)
        //    : base(storefrontContext)
        //{
        //}

        /// <summary>
        /// Gets or sets the line associated line ids.
        /// </summary>
        /// <value>
        /// The associated line ids.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IDs")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<string> LineIDs { get; protected set; }

        /// <summary>
        /// Gets or sets the party identifier.
        /// </summary>
        /// <value>
        /// The party identifier.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public string PartyID { get; set; }

        /// <summary>
        /// Gets or sets the shipping method identifier.
        /// </summary>
        /// <value>
        /// The shipping method identifier.
        /// </value>
        public string ShippingMethodId { get; set; }

        /// <summary>
        /// Gets or sets the name of the shipping method.
        /// </summary>
        /// <value>
        /// The name of the shipping method.
        /// </value>
        public string ShippingMethodName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the content of the email.
        /// </summary>
        /// <value>
        /// The content of the email.
        /// </value>
        public string EmailContent { get; set; }

        /// <summary>
        /// Gets or sets the type of the edit mode shipping option.
        /// </summary>
        /// <value>
        /// The type of the edit mode shipping option.
        /// </value>
        //public Sitecore.Commerce.Connect.Core.Models.ShippingOptionType EditModeShippingOptionType { get; set; }

        /// <summary>
        /// Gets or sets the shipment edit model.
        /// </summary>
        /// <value>
        /// The shipment edit model.
        /// </value>
        //public ShipmentEditModeDataJsonResult ShipmentEditModel { get; set; }

        /// <summary>
        /// Initializes the specified shipping information.
        /// </summary>
        /// <param name="shippingInfo">The shipping information.</param>
        //public virtual void Initialize(ShippingInfo shippingInfo)
        //{
        //    this.LineIDs = shippingInfo.LineIDs.ToList();
        //    this.PartyID = shippingInfo.PartyID;
        //    this.ShippingMethodId = shippingInfo.ShippingMethodID;

        //    if (!string.IsNullOrEmpty(this.ShippingMethodId))
        //    {
        //        var item = Sitecore.Context.Database.GetItem(ID.Parse(this.ShippingMethodId));
        //        if (item != null)
        //        {
        //            this.ShippingMethodName = this.StorefrontContext.GetShippingName(item.Name);
        //        }
        //    }
        //}
    }
}