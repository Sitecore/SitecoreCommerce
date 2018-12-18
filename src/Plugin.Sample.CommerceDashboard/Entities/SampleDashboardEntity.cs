namespace Plugin.Sample.CommerceDashboard.Entities
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;

    public class SampleDashboardEntity : CommerceEntity
    {
        public SampleDashboardEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }
    }
}