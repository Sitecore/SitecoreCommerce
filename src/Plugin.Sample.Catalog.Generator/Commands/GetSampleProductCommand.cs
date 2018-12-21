namespace Plugin.Sample.Catalog.Generator.Commands
{
    using System;
    using System.Collections.Generic;

    using Bogus;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Catalog;
    
    public class GetSampleProductCommand : CommerceCommand
    {
        private static Faker<SellableItem> _fakeSellableItem;

        public GetSampleProductCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Faker<SellableItem> CurrentFakerSellableItem()
        {
            if (_fakeSellableItem == null)
            { 
                _fakeSellableItem = new Faker<SellableItem>()
                    .RuleFor(u => u.Name, f => f.Commerce.ProductName())
                    .RuleFor(u => u.Tags, f => new List<Tag>() { new Tag("Generated") })
                    .FinishWith((f, u) =>
                    {
                        u.DisplayName = u.Name;
                    });
            }

            return _fakeSellableItem;
        }
        
        public SellableItem Process(CommerceContext commerceContext, List<Policy> policies = null)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var sellableItem = this.CurrentFakerSellableItem().Generate();

                return sellableItem;
            }
        }
    }
}