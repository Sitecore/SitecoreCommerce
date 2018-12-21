namespace Plugin.Sample.Catalog.Generator.Commands
{
    using System;
    using System.Collections.Generic;

    using Bogus;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Catalog;
    
    public class GetSampleCategoryCommand : CommerceCommand
    {
        private static Faker<Category> _fake;
        
        public GetSampleCategoryCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Faker<Category> CurrentFakerSellableItem()
        {
            if (_fake == null)
            {
                _fake = new Faker<Category>()
                    .RuleFor(u => u.Name, f => f.Commerce.Categories(1)[0] + "_" + f.Random.AlphaNumeric(4))
                    .FinishWith((f, u) =>
                    {
                        u.DisplayName = u.Name;
                    });
            }

            return _fake;
        }

        public Category Process(CommerceContext commerceContext, List<Policy> policies = null)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var sellableItem = this.CurrentFakerSellableItem().Generate();

                return sellableItem;
            }
        }
    }
}