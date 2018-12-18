namespace Plugin.Sample.Messaging.Entities
{
    using System;
    using System.Collections.Generic;

    using global::Plugin.Sample.Messaging.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ManagedLists;
    
    public class MessageEntity : CommerceEntity
    {
        public MessageEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
            this.History = new List<HistoryEntryModel>();
            this.Components.Add(new ListMembershipsComponent());
            this.TargetRoles = new List<string>();
            this.TargetUsers = new List<string>();
            this.Models = new List<Model>();
        }

        public List<HistoryEntryModel> History {get; set;}

        public List<string> TargetRoles { get; set; }

        public List<string> TargetUsers { get; set; }

        public List<Model> Models { get; set; }
    }
}