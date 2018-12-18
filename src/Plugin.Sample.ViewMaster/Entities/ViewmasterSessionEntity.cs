namespace Plugin.Sample.ViewMaster.Entities
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;

    public class ViewMasterSessionEntity : CommerceEntity
    {
        public ViewMasterSessionEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
            this.Views = new List<ViewMasterViewRecord>();
        }
        
        public ViewMasterSessionEntity(string id) : this()
        {
            this.Id = id;
        }
        
        public List<ViewMasterViewRecord> Views { get; set; }
    }
}