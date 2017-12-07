
using System;
using System.Collections.Generic;

using Bogus;

namespace Plugin.Sample.Promotions.Generator
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    /// <summary>
    /// Defines the GetSamplePromotionBookCommand command.
    /// </summary>
    public class GetSamplePromotionBookCommand : CommerceCommand
    {
        private static Faker<PromotionBookSample> _fake;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSamplePromotionBookCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        public GetSamplePromotionBookCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        /// <summary>
        /// Singleton pattern access to a Faker
        /// </summary>
        /// <returns></returns>
        public Faker<PromotionBookSample> CurrentFakerPromotion()
        {
            if (_fake == null)
            {
                //Randomizer.Seed = new Random();

                var productName = new Faker().Name;

                _fake = new Faker<PromotionBookSample>()
                        //Optional: Call for objects that have complex initialization
                        //.CustomInstantiator(f => new RegisteredHabitatCustomerDana(userIds++, f.Random.Replace("###-##-####")))

                        //Basic rules using built-in generators
                        .RuleFor(u => u.Name, f => Guid.NewGuid().ToString("N"))
                        .RuleFor(u => u.PublicCouponCode, f => f.Random.AlphaNumeric(29))
                        .RuleFor(u => u.PrivateCouponPrefix, f => Guid.NewGuid().ToString("N"))
                        .RuleFor(u => u.PrivateCouponSuffix, f => Guid.NewGuid().ToString("N"))
                        //.RuleFor(u => u.AccountNumber, f => f.Finance.Account(8))
                        .RuleFor(u => u.Description, f => f.Lorem.Sentence())
                        .RuleFor(u => u.DisplayText, f => f.Lorem.Sentence())
                        .RuleFor(u => u.DisplayCartText, f => f.Lorem.Sentence())
                        //.RuleFor(u => u.Password, f => "Boo!")

                        //.RuleFor(u => u.Tags, f => new List<Tag>() { new Tag("Generated") })

                        //.RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                        //.RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                        //.RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                        //.RuleFor(u => u.SomethingUnique, f => $"Value {f.UniqueIndex}")
                        //.RuleFor(u => u.SomeGuid, Guid.NewGuid)

                        //Use a method outside scope.
                        //.RuleFor(u => u.CartId, f => Guid.NewGuid())

                        //Compound property with context, use the first/last name properties
                        //.RuleFor(u => u.FullName, (f, u) => u.FirstName + " " + u.LastName)
                        //And composability of a complex collection.
                        //.RuleFor(u => u.Orders, f => testOrders.Generate(3).ToList())
                        //Optional: After all rules are applied finish with the following action
                        .FinishWith((f, u) =>
                        {
                            //Console.WriteLine($"BogusCustomer FirstName={u.FirstName}|LastName={u.LastName}");
                            u.DisplayName = u.Name;
                        });

            }
            return _fake;
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="policies">
        /// The policies for the command
        /// </param>
        /// <returns>
        /// The <see cref="ScCategory"/>.
        /// </returns>
        public PromotionBookSample Process(CommerceContext commerceContext, List<Policy> policies = null)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var sellableItem = this.CurrentFakerPromotion().Generate();

                return sellableItem;
            }
        }
    }
}