
namespace Plugin.Sample.Pricing.Generator.Models
{
    using System;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    
    public class PriceCardSample : Model
    {
        public PriceCardSample()
        {
            this.Id = string.Empty;
            this.Description = string.Empty;
            this.DisplayName = string.Empty;
            this.IndexId = Guid.NewGuid().ToString();
        }
        
        public string Id { get; private set; }
        
        public string IndexId { get; set; }

        public EntityReference Book { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool IsApproved(CommerceContext context)
        {
            Condition.Requires(context, nameof(context)).IsNotNull();
            return true;
        }

        public bool IsDraft(CommerceContext context)
        {
            Condition.Requires(context, nameof(context)).IsNotNull();
            
            return true;
        }
    }
}
