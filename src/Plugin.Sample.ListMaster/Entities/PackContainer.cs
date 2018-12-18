namespace Plugin.Sample.ListMaster.Entities
{
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ManagedLists;

    public class PackContainer : CommerceEntity
    {
        public PackContainer()
        {
            this.Entities = new List<CommerceEntity>();
            this.ManagedLists = new List<ManagedList>();
        }

        public List<CommerceEntity> Entities { get; set; }

        public List<ManagedList> ManagedLists { get; set; }
    }
}