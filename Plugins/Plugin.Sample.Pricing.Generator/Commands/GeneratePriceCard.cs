
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Plugin.Sample.Pricing.Generator
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Commerce.Plugin.Promotions;
    using System;
    using System.Linq;

    /// <summary>
    /// Defines the GeneratePriceCard command.
    /// </summary>
    public class GeneratePriceCard : CommerceCommand
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratePriceCard"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public GeneratePriceCard(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="bookName">
        /// The promotion
        /// </param>
        /// <param name="priceCardName">
        /// The priceCardName
        /// </param>
        /// <param name="priceCardDisplayName">
        /// The priceCardDisplayName
        /// </param>
        /// <param name="priceCardDescription">
        /// The priceCardDescription
        /// </param>
        /// <returns>
        /// The <see cref="PriceCard"/>.
        /// </returns>
        public async Task<PriceCard> Process(CommerceContext commerceContext, string bookName, string priceCardName, string priceCardDisplayName, string priceCardDescription)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var createPriceCardCommand = this._commerceCommander.Command<AddPriceCardCommand>();
                var createSnapshotCommand = this._commerceCommander.Command<AddPriceSnapshotCommand>();
                var createPriceTierCommand = this._commerceCommander.Command<AddPriceTierCommand>();

                PriceCard createdPriceCard = null;

                await this.PerformTransaction(
                    commerceContext,
                    async () =>
                    {
                        createdPriceCard = await createPriceCardCommand
                        .Process(commerceContext, bookName, priceCardName, priceCardDisplayName, priceCardDescription);

                        if (createdPriceCard == null)
                        {

                        }
                        else
                        {
                            var snapshot = new PriceSnapshotComponent(DateTime.UtcNow);

                            createdPriceCard.Snapshots.Add(snapshot);

                            snapshot.Tiers.Add(new PriceTier("USD", 1, 9.99M));

                            //createdPriceCard = await createSnapshotCommand
                            //    .Process(commerceContext, createdPriceCard, new PriceSnapshotComponent(DateTime.UtcNow));


                            //var createdPriceSnapshot = createdPriceCard.Snapshots.First();

                            //var createdPriceTier = await createPriceTierCommand
                            //    .Process(commerceContext, createdPriceCard, createdPriceSnapshot, new PriceTier("USD", 1, 9.99M));

                            await this._commerceCommander.PersistEntity(commerceContext, createdPriceCard);

                        }
                    });

                return createdPriceCard;
            }
        }
    }
}