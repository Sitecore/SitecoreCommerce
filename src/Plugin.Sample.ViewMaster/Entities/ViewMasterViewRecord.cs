namespace Plugin.Sample.ViewMaster.Entities
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    
    public class ViewMasterViewRecord : CommerceEntity
    {
        public ViewMasterViewRecord()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
            this.View = new EntityView();
            this.Headers = new Dictionary<string, string>();
        }

        public ViewMasterViewRecord(string id): this()
        {
            this.Id = id;
        }
        
        public EntityView View { get; set; }

        public Dictionary<string, string> Headers { get; set; }
        
        public bool NotNull(Func<bool> func)
        {
            if (this == null)
            {
                return false;
            }

            return true;
        }
    }
}