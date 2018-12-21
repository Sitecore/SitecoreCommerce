namespace Plugin.Sample.Promotions.Generator.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bogus;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.Promotions.Generator.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Plugin.Rules;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Framework.Rules;

    [PipelineDisplayName("DoActionGenerateSamplePromotionBook")]
    public class DoActionGenerateSamplePromotionBook : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionGenerateSamplePromotionBook(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Promotions-GenerateSamplePromotionBook", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var name = entityView.Properties.First(p => p.Name == "Name").Value ?? "";
                var displayName = entityView.Properties.First(p => p.Name == "DisplayName").Value ?? "";
                var promotions = entityView.Properties.First(p => p.Name == "Promotions").Value ?? "";

                var randomizer = new Random();
                Randomizer.Seed = randomizer;

                await this._commerceCommander.Command<GetActionsCommand>().Process(context.CommerceContext, typeof(IAction));

                var sellableItems = await this._commerceCommander.Command<ListCommander>()
                    .GetListItems<SellableItem>(context.CommerceContext, "SellableItems", 0, 500);
                var sellableItemsList = sellableItems.ToList();

                var fakePromotionBook = this._commerceCommander.Command<GetSamplePromotionBookCommand>().Process(context.CommerceContext, null);

                var createdPromotionBook = await this._commerceCommander.Command<AddPromotionBookCommand>().Process(context.CommerceContext, name, displayName, fakePromotionBook.Description, "");

                var createPromotionCommand = this._commerceCommander.Command<AddPromotionCommand>();

                var promotionsRequested = System.Convert.ToInt32(promotions);
                for (int i = 1; i <= promotionsRequested; i++)
                {
                    var fakePromotion = this._commerceCommander.Command<GetSamplePromotionCommand>().Process(context.CommerceContext, null);
                    var createdPromotion = await createPromotionCommand
                        .Process(context.CommerceContext, createdPromotionBook.Name, fakePromotion.Name, fakePromotion.ValidFrom, fakePromotion.ValidTo, fakePromotion.DisplayText, fakePromotion.DisplayCartText, fakePromotion.DisplayName, fakePromotion.Description, false);

                    context.CommerceContext.GetMessages().Add(new CommandMessage { Code = "Information", CommerceTermKey = "PromotionCreated", Text = $"Promotion Created" });
                    
                    var promotionItemsDesired = randomizer.Next(2, 20);
                    for (int j = 1; j <= promotionItemsDesired; j++)
                    {
                        var selectedSellableItem = sellableItemsList[new Random().Next(sellableItemsList.Count())];
                        var catalogName = selectedSellableItem.GetComponent<CatalogComponent>().Name;

                        //TODO - Correct Catalog Name
                        if (catalogName == "Habitat_Master")
                        {
                            catalogName = "Habitat_Master";
                        }
                        var sellableItemId = selectedSellableItem.Id.Replace("Entity-SellableItem-", "");

                        var itemVariations = selectedSellableItem.GetComponent<ItemVariationsComponent>();
                        var firstItemVariation = itemVariations.ChildComponents.OfType<ItemVariationComponent>().FirstOrDefault();

                        var itemVariationId = string.Empty;
                        if (firstItemVariation != null)
                        {
                            itemVariationId = firstItemVariation.Id;
                        }

                        if (string.IsNullOrEmpty(catalogName))
                        {
                            continue;
                        }
                        
                        var promotionItemId = $"{catalogName}|{sellableItemId}|{itemVariationId}";

                        createdPromotion = await this._commerceCommander.GetEntity<Promotion>(context.CommerceContext, $"{createdPromotion.Id}");
                        if (createdPromotion.GetComponent<PromotionItemsComponent>().Items.All(p => p.ItemId != promotionItemId))
                        {
                            createdPromotion = await this._commerceCommander.Command<AddPromotionItemCommand>()
                                .Process(context.CommerceContext, createdPromotion, promotionItemId, false);

                            context.CommerceContext.GetMessages().Add(new CommandMessage { Code = "Information", CommerceTermKey = "PromotionItemAdded", Text = $"Promotion Item Added" });
                        }
                    }

                    if (!createdPromotion.HasComponent<PromotionItemsComponent>())
                    {
                        continue;
                    }

                    var availableCartLineActions = await this._commerceCommander.Command<GetActionsCommand>()
                        .Process(context.CommerceContext, typeof(ICartLineAction));

                    var availableCartLinesActionsList = availableCartLineActions.ToList();
                    var selectedAction = availableCartLinesActionsList[randomizer.Next(0, availableCartLinesActionsList.Count-1)];

                    foreach(var property in selectedAction.Properties)
                    {
                        switch (property.Name)
                        {
                            case "Subtotal":
                                property.Value = randomizer.Next(10, 1000).ToString();
                                break;
                            case "Operator":

                                switch(randomizer.Next(0, 6).ToString())
                                {
                                    case "0":
                                        property.Value = "Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator";
                                        break;
                                    case "1":
                                        property.Value = new Sitecore.Framework.Rules.DateTimeEqualityOperator().GetType().FullName;
                                        break;
                                    case "2":
                                        property.Value = new Sitecore.Framework.Rules.DateTimeGreaterThanOperator().GetType().FullName;
                                        break;
                                    case "3":
                                        property.Value = new Sitecore.Framework.Rules.DateTimeLessThanEqualToOperator().GetType().FullName;
                                        break;
                                    case "4":
                                        property.Value = new Sitecore.Framework.Rules.DateTimeLessThanOperator().GetType().FullName;
                                        break;
                                    case "5":
                                        property.Value = new Sitecore.Framework.Rules.DecimalGreaterThanEqualToOperator().GetType().FullName;
                                        break;
                                }
                                break;
                            case "AmountOff":
                                property.Value = randomizer.Next(10, 90).ToString();
                                break;
                            case "PercentOff":
                                property.Value = randomizer.Next(10, 80).ToString();
                                break;
                            case "TargetItemId":
                                property.Value = "Habitat_Master|6042325|56042325";
                                break;
                            case "SellPrice":
                                property.Value = "99.99";
                                break;
                        }
                    }

                    var promotionBenefitAdded = await this._commerceCommander.Command<AddBenefitCommand>()
                        .Process(context.CommerceContext.GetPipelineContextOptions().CommerceContext, createdPromotion.Id, selectedAction);

                    if (promotionBenefitAdded == null)
                    {
                        promotionBenefitAdded = createdPromotion;
                    }

                    promotionBenefitAdded.DisplayName = selectedAction.Name;
                    
                    if (context.CommerceContext.GetMessages().Any(p=>p.Code == "Error"))
                    {
                        foreach(var message in context.CommerceContext.GetMessages())
                        {
                            var messageText = $"PromotionGenerator.AddBenefit.{message.Code}: CommerceTermKey={message.CommerceTermKey}|MessageDate={message.MessageDate}|Text={message.Text}|ActionId={selectedAction.Id}|ActionId={selectedAction.LibraryId}|ActionName={selectedAction.Name}";
                            context.Logger.LogWarning(messageText);

                            promotionBenefitAdded.GetComponent<MessagesComponent>().AddMessage(
                                context.GetPolicy<KnownMessageCodePolicy>().Promotions,
                                messageText);
                        }

                        context.CommerceContext.GetMessages().RemoveAll(p => p.Code == "Error");
                    }

                    var result = await this._commerceCommander.PersistEntity(context.CommerceContext, promotionBenefitAdded);
                    if (!result)
                    {
                    }
                    else
                    {
                        context.CommerceContext.GetMessages().Add(new CommandMessage { Code = "Information", CommerceTermKey = "PromotionBenefitAdded", Text = $"Promotion Benefit Added ({selectedAction.Name})" });
                    }

                    await this._commerceCommander.Command<ApproveGeneratedPromotion>().Process(context.CommerceContext, createdPromotion);
                    
                    context.CommerceContext.GetMessages().Clear();
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Promotions.DoActionGeneratorSamplePromotionBook.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
