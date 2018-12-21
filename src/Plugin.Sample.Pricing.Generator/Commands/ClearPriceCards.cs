namespace Plugin.Sample.Pricing.Generator.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Pricing;

    public class ClearPriceCards : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;
        
        public ClearPriceCards(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public async Task<PriceCard> Process(CommerceContext commerceContext, string bookId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                PriceCard createdPriceCard = null;
                var bookName = bookId.Replace("Entity-PriceBook-", "");
                var listName = string.Format(commerceContext.GetPolicy<KnownPricingListsPolicy>().PriceBookCards, bookName);

                var cardIds = await this._commerceCommander.Command<ListCommander>()
                    .GetListItemIds<PriceCard>(commerceContext, listName, 0, 100);

                while (cardIds.Count > 0)
                {
                    var tasks = new List<Task>();

                    foreach (var card in cardIds)
                    {
                        tasks.Add(Task.Run(() => this._commerceCommander
                            .DeleteEntity(commerceContext, card)));
                    }

                    await Task.WhenAll(tasks);

                    cardIds = await this._commerceCommander.Command<ListCommander>()
                        .GetListItemIds<PriceCard>(commerceContext, listName, 0, 100);
                }

                return createdPriceCard;
            }
        }
    }
}