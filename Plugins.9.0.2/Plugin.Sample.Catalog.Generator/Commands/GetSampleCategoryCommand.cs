// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSampleCategoryCommand.cs" company="Sitecore Corporation">
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
    /// Defines the GetSampleCategoryCommand command.
    /// </summary>
    public class GetSampleCategoryCommand : CommerceCommand
    {
        private static Faker<Category> _fake;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSampleCategoryCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        public GetSampleCategoryCommand(IServiceProvider serviceProvider) : base(serviceProvider)
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
        public Faker<Category> CurrentFakerSellableItem()
        {
            if (_fake == null)
            {
                //Randomizer.Seed = new Random();

                var productName = new Faker().Name;

                _fake = new Faker<Category>()
                        //Optional: Call for objects that have complex initialization
                        //.CustomInstantiator(f => new RegisteredHabitatCustomerDana(userIds++, f.Random.Replace("###-##-####")))

                        //Basic rules using built-in generators
                        .RuleFor(u => u.Name, f => f.Commerce.Categories(1)[0] + "_" + f.Random.AlphaNumeric(4))
                        //.RuleFor(u => u.DisplayName, f => f.Commerce.ProductName())
                        //.RuleFor(u => u.AccountNumber, f => f.Finance.Account(8))
                        //.RuleFor(u => u.AccountStatus, f => "ActiveAccount")
                        //.RuleFor(u => u.Password, f => "Boo!")

                        //.RuleFor(u => u.Tags, f => new List<Tag>() { new Tag("Generated") })

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
        /// The <see cref="Category"/>.
        /// </returns>
        public Category Process(CommerceContext commerceContext, List<Policy> policies = null)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var sellableItem = this.CurrentFakerSellableItem().Generate();

                return sellableItem;
            }
        }
    }
}