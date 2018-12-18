namespace Plugin.Sample.VatTax.Entities
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;
    
    public class VatTaxTableEntity : CommerceEntity
    {
        public VatTaxTableEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
            this.CountryCode = "US";
            this.TaxTag = string.Empty;
            this.TaxPct = 0M;
        }

        public VatTaxTableEntity(string id): this()
        {
            this.Id = id;
        }
        
        public string CountryCode { get; set; }
        
        public string TaxTag { get; set; }
        
        public decimal TaxPct { get; set; }
    }
}