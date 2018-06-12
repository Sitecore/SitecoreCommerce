//-----------------------------------------------------------------------
// <copyright file="CartLineJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>Defines the CartLineJsonResult class.</summary>
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
    using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;
    //using Diagnostics;
    //using Entities.Carts;
    //using Foundation.Common;
    //using Foundation.Common.ExtensionMethods;
    //using Foundation.Common.Models;
    //using Foundation.Common.Models.JsonResults;
    //using Foundation.Connect.Managers;
    //using Links;

    /// <summary>
    /// Cart Line Json object
    /// </summary>
    public class CartLineJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartLineJsonResult"/> class.
        /// </summary>
        /// <param name="storefrontContext">the storefront context</param>
        /// <param name="modelProvider">the model provider</param>
        /// <param name="searchManager">search manager</param>
        //public CartLineJsonResult([NotNull]IStorefrontContext storefrontContext, [NotNull]IModelProvider modelProvider, [NotNull] ISearchManager searchManager)
        //    : base(storefrontContext)
        //{
        //    Assert.ArgumentNotNull(modelProvider, "modelProvider");
        //    Assert.ArgumentNotNull(searchManager, "searchManager");

        //    this.ModelProvider = modelProvider;
        //    this.SearchManager = searchManager;
        //}

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
        /// Gets or sets the search manager
        /// </summary>
        //[ScriptIgnore]
        //public ISearchManager SearchManager
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Gets or sets the external cart line identifier.
        /// </summary>
        /// <value>
        /// The external cart line identifier.
        /// </value>
        public string ExternalCartLineId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public string Image
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the product url.
        /// </summary>
        /// <value>
        /// The product url.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ProductUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color variant information.
        /// </summary>
        /// <value>
        /// The variant information.
        /// </value>
        public string ColorInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size variant information.
        /// </summary>
        /// <value>
        /// The variant information.
        /// </value>
        public string SizeInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the style variant information.
        /// </summary>
        /// <value>
        /// The variant information.
        /// </value>
        public string StyleInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the gift card amount information.
        /// </summary>
        /// <value>
        /// The gift card amount information.
        /// </value>
        public string GiftCardAmountInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public string Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the line price.
        /// </summary>
        /// <value>
        /// The line price.
        /// </value>
        public string LinePrice
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the line total.
        /// </summary>
        /// <value>
        /// The line total.
        /// </value>
        public string LineTotal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the line shipping options.
        /// </summary>
        /// <value>The line shipping options.</value>
        public IEnumerable<ShippingOptionJsonResult> ShippingOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the discount offer names.
        /// </summary>
        /// <value>
        /// The discount offer names.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<string> DiscountOfferNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the line discount.
        /// </summary>
        /// <value>
        /// The line discount.
        /// </value>
        public string LineDiscount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the product id for current cart line
        /// </summary>
        public string ProductId
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the specified cart line.
        /// </summary>
        /// <param name="cartLine">The cart line.</param>
        //public virtual void Initialize(CartLine cartLine)
        //{
        //    this.ExternalCartLineId = cartLine.ExternalCartLineId;
        //    this.ProductId = cartLine.Product.ProductId;
        //    this.Quantity = cartLine.Quantity.ToString(Context.Language.CultureInfo);
        //    this.LinePrice = cartLine.Product.Price.Amount.ToCurrency();
        //    this.LineTotal = cartLine.Total.Amount.ToCurrency();
        //    this.SetLink();
        //    this.DiscountOfferNames = new List<string>();
        //    var lineDiscount = (decimal)0.0;
        //    if (cartLine.Adjustments.Count > 0)
        //    {
        //        foreach (var adjustment in cartLine.Adjustments)
        //        {
        //            this.DiscountOfferNames.Add(adjustment.Description);
        //            lineDiscount = lineDiscount + adjustment.Amount;
        //        }
        //    }

        //    this.LineDiscount = lineDiscount.ToCurrency();
        //}

        /// <summary>
        /// Sets the shipping options.
        /// </summary>
        /// <param name="shippingOptions">The shipping options.</param>
        public virtual void SetShippingOptions(IEnumerable<ShippingOptionJsonResult> shippingOptions)
        {
            if (shippingOptions == null)
            {
                return;
            }

            this.ShippingOptions = new List<ShippingOptionJsonResult>(shippingOptions);
        }

        /// <summary>
        /// Sets the product link for current item
        /// </summary>
        //public virtual void SetLink()
        //{
        //    var productItem = this.SearchManager.GetProduct(this.ProductId, this.StorefrontContext.CurrentStorefront.Catalog);
        //    var giftCardProductId = this.StorefrontContext.CurrentStorefront.GiftCardProductId;

        //    this.ProductUrl = this.ProductId.Equals(giftCardProductId, StringComparison.OrdinalIgnoreCase)
        //        ? this.StorefrontContext.CurrentStorefront.GiftCardPageLink
        //        : LinkManager.GetDynamicUrl(productItem);
        //}
    }
}