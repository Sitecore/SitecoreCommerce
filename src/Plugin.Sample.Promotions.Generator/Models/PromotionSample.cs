
namespace Plugin.Sample.Promotions.Generator.Models
{
    using System;

    using Sitecore.Commerce.Core;
    
    public class PromotionSample : Model
    {
        public PromotionSample()
        {
            this.Id = string.Empty;
            this.Description = string.Empty;
            this.DisplayText = string.Empty;
            this.DisplayName = string.Empty;
            this.DisplayCartText = string.Empty;
            this.ValidFrom = DateTimeOffset.UtcNow.AddDays(-10);
            this.ValidTo = DateTimeOffset.UtcNow.AddDays(10);
            this.IndexId = Guid.NewGuid().ToString();
        }
        
        public string PrivateCouponPrefix { get; set; }
        
        public string PrivateCouponSuffix { get; set; }
        
        public string PublicCouponCode { get; set; }
        
        public string Id { get; private set; }
        
        public string IndexId { get; set; }
        
        public EntityReference Book { get; set; }

        public string DisplayText { get; set; }
        
        public string DisplayName { get; set; }

        public string DisplayCartText { get; set; }

        public DateTimeOffset ValidFrom { get; set; }

        public DateTimeOffset ValidTo { get; set; }

        public string Description { get; set; }
    }
}
