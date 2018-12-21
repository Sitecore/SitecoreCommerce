namespace Plugin.Sample.Pricing.Generator.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bogus;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.Pricing.Generator.Commands;
    using Plugin.Sample.Pricing.Generator.Components;
    using Plugin.Sample.Pricing.Generator.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionGenerateSamplePriceBook")]
    public class DoActionGenerateSamplePriceBook : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionGenerateSamplePriceBook(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

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
                var createdPriceBook = await this._commerceCommander
                    .Command<AddPriceBookCommand>()
                    .Process(context.CommerceContext, name, name, name, "");

                var priceCardsRequested = System.Convert.ToInt32(priceCards);

                var generatePriceCardCommand = this._commerceCommander.Command<GeneratePriceCard>();

                var tasks = new List<Task>();

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

                await this._commerceCommander.PersistEntity(context.CommerceContext, createdPriceBook);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Pricing.DoActionGeneratorSamplePriceBook.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
