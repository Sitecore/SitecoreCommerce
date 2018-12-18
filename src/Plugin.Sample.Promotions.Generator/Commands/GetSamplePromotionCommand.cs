namespace Plugin.Sample.Promotions.Generator.Commands
{
    using System;
    using System.Collections.Generic;

    using Bogus;

    using Plugin.Sample.Promotions.Generator.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    
    public class GetSamplePromotionCommand : CommerceCommand
    {
        private static Faker<PromotionSample> _fake;
        
        public GetSamplePromotionCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        
        public Faker<PromotionSample> CurrentFakerPromotion()
        {
            if (_fake == null)
            {
                _fake = new Faker<PromotionSample>()
                    .RuleFor(u => u.Name, f => Guid.NewGuid().ToString("N"))
                    .RuleFor(u => u.DisplayName, f => f.Random.Words(4))
                    .RuleFor(u => u.PublicCouponCode, f => f.Random.AlphaNumeric(29))
                    .RuleFor(u => u.PrivateCouponPrefix, f => Guid.NewGuid().ToString("N"))
                    .RuleFor(u => u.PrivateCouponSuffix, f => Guid.NewGuid().ToString("N"))
                    .RuleFor(u => u.Description, f => f.Lorem.Sentence())
                    .RuleFor(u => u.DisplayText, f => f.Lorem.Sentence())
                    .RuleFor(u => u.DisplayCartText, f => f.Lorem.Sentence())
                    .FinishWith((f, u) =>
                    {
                    });
            }

            return _fake;
        }

        public PromotionSample Process(CommerceContext commerceContext, List<Policy> policies = null)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var sellableItem = this.CurrentFakerPromotion().Generate();

                return sellableItem;
            }
        }
    }
}