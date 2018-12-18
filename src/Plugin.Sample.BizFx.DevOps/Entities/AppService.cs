namespace Plugin.Sample.BizFx.DevOps.Entities
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;

    public class AppService : CommerceEntity
    {
        public AppService()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }
        
        public AppService(string id): this()
        {
            this.Id = id;
        }

        public string ServiceType { get; set; }
        
        public string Host { get; set; }

        public string Description { get; set; }
    }
}