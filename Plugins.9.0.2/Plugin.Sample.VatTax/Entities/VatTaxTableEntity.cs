// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VatTaxTableEntity.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   SampleEntity model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.VatTax
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.OData.Builder;
    using Sitecore.Commerce.Core;

    /// <summary>
    /// SampleDashboardEntity model.
    /// </summary>
    public class VatTaxTableEntity : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VatTaxTableEntity"/> class.
        /// </summary>
        public VatTaxTableEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
            this.CountryCode = "US";
            this.TaxTag = "";
            this.TaxPct = 0M;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VatTaxTableEntity"/> class. 
        /// Public Constructor
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public VatTaxTableEntity(string id): this()
        {
            this.Id = id;
        }

        /// <summary>
        /// Country Code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Tax Tag
        /// </summary>
        public string TaxTag { get; set; }

        /// <summary>
        /// Tax Pct
        /// </summary>
        public decimal TaxPct { get; set; }

        ///// <summary>
        ///// Gets or sets the list of child components in the SampleEntity
        ///// </summary>
        //[Contained]
        //public IEnumerable<SampleComponent> ChildComponents { get; set; }
    }
}