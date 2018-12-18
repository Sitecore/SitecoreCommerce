namespace Plugin.Sample.Pricing.Generator.Commands
{
    using System;
    using System.Collections.Generic;

    using Bogus;

    using Plugin.Sample.Pricing.Generator.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class GetSamplePriceCardCommand : CommerceCommand
    {
        private static Faker<PriceCardSample> _fake;

        public GetSamplePriceCardCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        
        public Faker<PriceCardSample> CurrentFakerPromotion()
        {
            if (_fake == null)
            {
                _fake = new Faker<PriceCardSample>()
                    .RuleFor(u => u.Name, f => f.Random.AlphaNumeric(29))
                    .RuleFor(u => u.DisplayName, f => f.Random.Words(4))
                    .RuleFor(u => u.Description, f => f.Lorem.Sentence())
                    .FinishWith((f, u) =>
                    {
                    });
            }

            return _fake;
        }

        public PriceCardSample Process(CommerceContext commerceContext, List<Policy> policies = null)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var sellableItem = this.CurrentFakerPromotion().Generate();

                return sellableItem;
            }
        }
    }
}