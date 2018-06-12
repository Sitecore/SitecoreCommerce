
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
    /// Defines the ClearPriceCards command.
    /// </summary>
    public class ClearPriceCards : CommerceCommand
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearPriceCards"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ClearPriceCards(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="bookId">
        /// The bookId
        /// </param>
        /// <returns>
        /// The <see cref="PriceCard"/>.
        /// </returns>
        public async Task<PriceCard> Process(CommerceContext commerceContext, string bookId)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {

                PriceCard createdPriceCard = null;

                //await this.PerformTransaction(
                //    commerceContext,
                //    async () =>
                //    {
                        var startTime = DateTime.UtcNow;
                        var bookName = bookId.Replace("Entity-PriceBook-", "");
                        var listName = string.Format(commerceContext.GetPolicy<KnownPricingListsPolicy>().PriceBookCards, bookName);

                        var cardIds = await this._commerceCommander.Command<ListCommander>()
                            .GetListItemIds<PriceCard>(commerceContext, listName, 0, 100);

                        while (cardIds.Count() > 0)
                        {
                            List<Task> tasks = new List<Task>();

                            foreach (var card in cardIds)
                            {
                                tasks.Add(Task.Run(() => this._commerceCommander
                                    .DeleteEntity(commerceContext, card)));
                            }

                            await Task.WhenAll(tasks);

                            cardIds = await this._commerceCommander.Command<ListCommander>()
                            .GetListItemIds<PriceCard>(commerceContext, listName, 0, 100);

                        }

                    //});

                return createdPriceCard;
            }
        }
    }
}