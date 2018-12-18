namespace Plugin.Sample.ListMaster.Entities
{
    using System;
    using System.Collections.Generic;

    using global::Plugin.Sample.ListMaster.Models;

    using Sitecore.Commerce.Core;
    
    public class ListTrackingEntity : CommerceEntity
    {
        public ListTrackingEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }

        public ListTrackingEntity(string id): this()
        {
            this.Id = id;
            this.Lists = new List<TrackedList>();
        }
        
        public List<TrackedList> Lists { get; set; }
    }
}