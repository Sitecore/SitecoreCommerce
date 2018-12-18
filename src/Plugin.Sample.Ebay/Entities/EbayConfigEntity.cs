namespace Plugin.Sample.Ebay.Entities
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;

    public class EbayConfigEntity : CommerceEntity
    {
        public EbayConfigEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }
        
        public EbayConfigEntity(string id) : this()
        {
            this.Id = id;
        }
    }
}