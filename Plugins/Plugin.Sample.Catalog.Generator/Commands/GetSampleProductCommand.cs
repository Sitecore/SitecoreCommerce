// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSampleProductCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the GetSampleCustomerCommand command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.CatalogGenerator
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Bogus;
    using System.Collections.Generic;
    using Sitecore.Commerce.Plugin.Catalog;

    /// <summary>
    /// Defines the GetSampleProductCommand command.
    /// </summary>
    public class GetSampleProductCommand : CommerceCommand
    {
        private static Faker<SellableItem> _fakeSellableItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSampleProductCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        public GetSampleProductCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        /// <summary>
        /// Gender ENum
        /// </summary>
        public enum Gender
        {
            /// <summary>
            /// Male
            /// </summary>
            Male,
            /// <summary>
            /// Femail
            /// </summary>
            Female
        }


        /// <summary>
        /// AccountStatus Enum
        /// </summary>
        public enum AccountStatus
        {
            /// <summary>
            /// ActiveAccount
            /// </summary>
            ActiveAccount,
            /// <summary>
            /// InactiveAccount
            /// </summary>
            InactiveAccount
        }

        /// <summary>
        /// Singleton pattern access to a Faker
        /// </summary>
        /// <returns></returns>
        public Faker<SellableItem> CurrentFakerSellableItem()
        {
            if (_fakeSellableItem == null)
            {
                //Randomizer.Seed = new Random();

                var productName = new Faker().Name;

                _fakeSellableItem = new Faker<SellableItem>()
                        //Optional: Call for objects that have complex initialization
                        //.CustomInstantiator(f => new RegisteredHabitatCustomerDana(userIds++, f.Random.Replace("###-##-####")))

                        //Basic rules using built-in generators
                        .RuleFor(u => u.Name, f => f.Commerce.ProductName())
                        //.RuleFor(u => u.DisplayName, f => f.Commerce.ProductName())
                        //.RuleFor(u => u.AccountNumber, f => f.Finance.Account(8))
                        //.RuleFor(u => u.AccountStatus, f => "ActiveAccount")
                        //.RuleFor(u => u.Password, f => "Boo!")

                        .RuleFor(u => u.Tags, f => new List<Tag>() { new Tag("Generated") })

                        //.RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                        //.RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                        //.RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                        //.RuleFor(u => u.SomethingUnique, f => $"Value {f.UniqueIndex}")
                        //.RuleFor(u => u.SomeGuid, Guid.NewGuid)

                        //Use an enum outside scope.
                        //.RuleFor(u => u.AccountStatus, f => f.PickRandom<AccountStatus>().ToString())
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
            return _fakeSellableItem;
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
        /// The <see cref="SellableItem"/>.
        /// </returns>
        public SellableItem Process(CommerceContext commerceContext, List<Policy> policies = null)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var sellableItem = this.CurrentFakerSellableItem().Generate();

                return sellableItem;
            }
        }
    }
}