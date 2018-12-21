namespace Plugin.Sample.Pricing.Generator.Commands
{
    using System;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Pricing;

    public class GeneratePriceCard : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;
        
        public GeneratePriceCard(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public async Task<PriceCard> Process(CommerceContext commerceContext, string bookName, string priceCardName, string priceCardDisplayName, string priceCardDescription)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var createPriceCardCommand = this._commerceCommander.Command<AddPriceCardCommand>();

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
                            await this._commerceCommander.PersistEntity(commerceContext, createdPriceCard);
                        }
                    });

                return createdPriceCard;
            }
        }
    }
}