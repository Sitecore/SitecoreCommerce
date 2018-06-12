
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Bogus;

namespace Plugin.Sample.Pricing.Generator
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Framework.Rules;
    using Sitecore.Commerce.Plugin.Rules;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Pricing;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionGenerateSamplePriceBook")]
    public class DoActionGenerateSamplePriceBook : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionGenerateSamplePriceBook"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionGenerateSamplePriceBook(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="entityView">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (entityView == null
                || !entityView.Action.Equals("Pricing-GenerateSamplePriceBook", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {

                var startTime = DateTime.UtcNow;

                var name = entityView.Properties.First(p => p.Name == "Name").Value ?? "";
                var priceCards = entityView.Properties.First(p => p.Name == "PriceCards").Value ?? "";

                var randomizer = new Random();
                Randomizer.Seed = randomizer;

                //var fakePriceBook = await this._commerceCommander
                //    .Command<GetSamplePriceBookCommand>()
                //    .Process(context.CommerceContext, name, null);

                var createdPriceBook = await this._commerceCommander
                    .Command<AddPriceBookCommand>()
                    .Process(context.CommerceContext, name, name, name, "");

                var createPriceCardCommand = this._commerceCommander.Command<AddPriceCardCommand>();
                var createSnapshotCommand = this._commerceCommander.Command<AddPriceSnapshotCommand>();
                var createPriceTierCommand = this._commerceCommander.Command<AddPriceTierCommand>();

                var priceCardsRequested = System.Convert.ToInt32(priceCards);

                var generatePriceCardCommand = this._commerceCommander.Command<GeneratePriceCard>();

                List<Task> tasks = new List<Task>();

                for (int i = 1; i <= priceCardsRequested; i++)
                {
                    try
                    {
                        var fakePriceCard = this._commerceCommander
                        .Command<GetSamplePriceCardCommand>()
                        .Process(context.CommerceContext, null);

                        tasks.Add(Task.Run(() => generatePriceCardCommand
                            .Process(context.CommerceContext, createdPriceBook.Name, fakePriceCard.Name,
                            fakePriceCard.DisplayName, fakePriceCard.Description)));

                        if (tasks.Count > 100)
                        {
                            await Task.WhenAll(tasks);
                            tasks = new List<Task>();
                        }
                    }
                    catch(Exception ex)
                    {
                        context.Logger.LogError($"Pricing.DoActionGeneratorSamplePriceBook.Exception: Message={ex.Message}");
                    }
                }
                await Task.WhenAll(tasks);

                createdPriceBook.GetComponent<ActionHistoryComponent>()
                    .AddHistory(new ActionHistoryModel
                    {
                        Name = entityView.Action,
                        StartTime = startTime,
                        Response = "Ok",
                        EntityId = entityView.EntityId,
                        ItemId = entityView.ItemId
                    });

                var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, createdPriceBook);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Pricing.DoActionGeneratorSamplePriceBook.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
