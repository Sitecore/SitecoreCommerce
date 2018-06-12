//-----------------------------------------------------------------------
// <copyright file="PaymentInfoJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>Defines the PaymentInfoJsonResult class.</summary>
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
    //using Entities.GiftCards;
    //using Entities.Payments;
    //using Foundation.Common;
    //using Foundation.Common.ExtensionMethods;
    using Foundation.Common.Models.JsonResults;
    using System.Collections.Generic;
    using System.Linq;

    public class PaymentInfoJsonResult : BaseJsonResult
    {
        //public PaymentInfoJsonResult([NotNull]IStorefrontContext storefrontContext)
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
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the type of the payment.
        /// </summary>
        /// <value>
        /// The type of the payment.
        /// </value>
        public int PaymentType { get; set; }

        /// <summary>
        /// Gets or sets the payment method identifier.
        /// </summary>
        /// <value>
        /// The payment method identifier.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Justification = "Sitecore standard for ID fields.")]
        public string PaymentMethodID { get; set; }

        /// <summary>
        /// Gets or sets the gift card number.
        /// </summary>
        /// <value>
        /// The gift card number.
        /// </value>
        public string GiftCardNumber { get; set; }

        /// <summary>
        /// Gets or sets the card token.
        /// </summary>
        /// <value>
        /// The card token.
        /// </value>
        public string CardToken { get; set; }

        /// <summary>
        /// Gets or sets the existing payment message.
        /// </summary>
        /// <value>
        /// The existing payment message.
        /// </value>
        public string ExistingPaymentMessage { get; set; }

        /// <summary>
        /// Gets or sets the cart amount different than payment message.
        /// </summary>
        /// <value>
        /// The cart amount different than payment message.
        /// </value>
        public string CartAmountDifferentThanPaymentMessage { get; set; }

        /// <summary>
        /// Initializes the specified shipping information.
        /// </summary>
        /// <param name="paymentInfo">The payment information.</param>
        //public virtual void Initialize(PaymentInfo paymentInfo)
        //{
        //    this.LineIDs = paymentInfo.LineIDs.ToList();
        //    this.PartyID = paymentInfo.PartyID;
        //    this.PaymentMethodID = paymentInfo.PaymentMethodID;

        //    if (paymentInfo is FederatedPaymentInfo)
        //    {
        //        this.Amount = ((FederatedPaymentInfo)paymentInfo).Amount;
        //        this.PaymentType = PaymentOptionType.PayFederatedPayment;
        //        this.CardToken = ((FederatedPaymentInfo)paymentInfo).CardToken;

        //        var currencyCode = this.StorefrontContext.CurrentStorefront.SelectedCurrency;

        //        this.ExistingPaymentMessage = this.StorefrontContext.GetSystemMessage(CartFeatureConstants.SystemMessages.ExistingPaymentText);
        //        this.CartAmountDifferentThanPaymentMessage = this.StorefrontContext.GetSystemMessage(CartFeatureConstants.SystemMessages.CartAmountDifferentPaymentText);
        //    }
        //    else if (paymentInfo is GiftCardPaymentInfo)
        //    {
        //        this.Amount = ((GiftCardPaymentInfo)paymentInfo).Amount;
        //        this.PaymentType = PaymentOptionType.PayGiftCard;
        //        this.GiftCardNumber = ((GiftCardPaymentInfo)paymentInfo).CardNumber;
        //    }
        //}
    }
}