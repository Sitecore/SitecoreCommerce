//-----------------------------------------------------------------------
// <copyright file="CartJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>Defines the CartJsonResult class.</summary>
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
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Serialization;
    //using Diagnostics;
    //using Entities.Carts;
    //using Foundation.Common;
    //using Foundation.Common.ExtensionMethods;
    //using Foundation.Common.Models;
    //using Foundation.Common.Models.JsonResults;

    /// <summary>
    /// Json result object containing cart information
    /// </summary>
    public class CartJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartJsonResult"/> class.
        /// </summary>
        /// <param name="storefrontContext">storefront context</param>
        /// <param name="modelProvider">model provider</param>
        public CartJsonResult()
           // : base(storefrontContext)
        {
            this.AccountingParty = new PartyLinkJsonResult { Name = "PartyName", PartyID = "PartyId" };
            this.Discount = "";
            this.Email = "";
            this.Lines = new List<CartLineJsonResult>();
            this.Parties = new List<AddressJsonResult>();
            this.Payments = new List<PaymentInfoJsonResult>();
            this.PromoCodes = new List<string>();
            this.Shipments = new List<ShippingInfoJsonResult>();
            this.ShippingTotal = "";
            this.Subtotal = "";
            this.TaxTotal = "";
            this.Total = "";
            this.TotalAmount = 0M;
            
            //Assert.ArgumentNotNull(modelProvider, "modelProvider");

            //this.ModelProvider = modelProvider;
        }

        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the model provider.
        /// </summary>
        /// <value>
        /// The model provider.
        /// </value>
        //[ScriptIgnore]
        //public IModelProvider ModelProvider
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Gets or sets the cart lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<CartLineJsonResult> Lines
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the sub total.
        /// </summary>
        /// <value>
        /// The sub total.
        /// </value>
        public string Subtotal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tax total.
        /// </summary>
        /// <value>
        /// The tax total.
        /// </value>
        public string TaxTotal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public string Total
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the shipping total.
        /// </summary>
        /// <value>
        /// The shipping total.
        /// </value>
        public string ShippingTotal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        /// <value>
        /// The total amount.
        /// </value>
        public decimal TotalAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the shipments.
        /// </summary>
        /// <value>
        /// The shipments.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<ShippingInfoJsonResult> Shipments
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        /// <value>
        /// The payments.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<PaymentInfoJsonResult> Payments
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the parties.
        /// </summary>
        /// <value>
        /// The parties.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<AddressJsonResult> Parties
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the accounting party.
        /// </summary>
        /// <value>
        /// The accounting party.
        /// </value>
        public PartyLinkJsonResult AccountingParty { get; set; }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        /// <value>
        /// The discount.
        /// </value>
        public string Discount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the promo codes for the cart
        /// </summary>
        public IEnumerable<string> PromoCodes
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the property values from the given cart
        /// </summary>
        /// <param name="cart">the cart</param>
        public virtual void Initialize(Sitecore.Commerce.Plugin.Carts.Cart cart)
        {
            var contactComponent = cart.GetComponent<ContactComponent>();
            var messages = cart.GetComponent<MessagesComponent>();

            this.Email = contactComponent.Email;

            //this.TaxTotal = cart.Totals..TaxTotal.Amount.ToCurrency();
            this.Total = cart.Totals.GrandTotal.AsCurrency();
            this.TotalAmount = cart.Totals.GrandTotal.Amount;
            this.Subtotal = cart.Totals.SubTotal.AsCurrency();

            //this.PromoCodes = cart.GetPropertyValue("PromoCodes") as IEnumerable<string>;

            foreach (var line in cart.Lines)
            {
                var cartProduct = line.GetComponent<CartProductComponent>();
                var itemVariationSelected = line.GetComponent<ItemVariationSelectedComponent>();
                var messagesComponent = line.GetComponent<MessagesComponent>();
                var itemAvailabilityComponent = line.GetComponent<ItemAvailabilityComponent>();

                var lineModel = new CartLineJsonResult { ColorInformation = cartProduct.Color, DisplayName = cartProduct.DisplayName,
                    ExternalCartLineId = line.ItemId,
                    GiftCardAmountInformation = "", Image = "", LineDiscount = "",
                    LinePrice = line.UnitListPrice.AsCurrency(),
                    LineTotal = line.Totals.GrandTotal.AsCurrency(),
                    ProductId = line.ItemId, ProductUrl = "", Quantity = line.Quantity.ToString(),
                    ShippingOptions = new List<ShippingOptionJsonResult>(),
                    DiscountOfferNames = new List<string>(),
                    SizeInformation = cartProduct.Size, StyleInformation = cartProduct.Style,
                    Success = true, Url = "",  };

                //lineModel.Initialize(line);

                this.Lines.Add(lineModel);
            }

            //if (cart.Shipping != null && cart.Shipping.Any())
            //{
            //    foreach (var shipping in cart.Shipping)
            //    {
            //        var shipmentModel = this.ModelProvider.GetModel<ShippingInfoJsonResult>();

            //        shipmentModel.Initialize(shipping);

            //        this.Shipments.Add(shipmentModel);
            //    }
            //}

            //this.Parties = new List<JsonResults.AddressJsonResult>();
            //if (cart.Parties != null && cart.Parties.Any())
            //{
            //    foreach (var party in cart.Parties)
            //    {
            //        var addressModel = this.ModelProvider.GetModel<AddressJsonResult>();

            //        addressModel.Initialize(party);
            //        this.Parties.Add(addressModel);
            //    }
            //}

            //this.Payments = new List<JsonResults.PaymentInfoJsonResult>();

            //if (cart.Payment != null && cart.Payment.Any())
            //{
            //    foreach (var payment in cart.Payment)
            //    {
            //        var paymentInfoModel = this.ModelProvider.GetModel<PaymentInfoJsonResult>();

            //        paymentInfoModel.Initialize(payment);

            //        this.Payments.Add(paymentInfoModel);
            //    }
            //}

            //if (cart.AccountingCustomerParty != null)
            //{
            //    this.AccountingParty = this.ModelProvider.GetModel<PartyLinkJsonResult>();
            //    this.AccountingParty.Name = cart.AccountingCustomerParty.Name;
            //    this.AccountingParty.PartyID = cart.AccountingCustomerParty.PartyID;
            //}
        }
    }
}