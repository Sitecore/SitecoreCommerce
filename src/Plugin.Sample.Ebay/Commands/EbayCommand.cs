namespace Plugin.Sample.Ebay.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using eBay.Service.Call;
    using eBay.Service.Core.Sdk;
    using eBay.Service.Core.Soap;

    using global::Plugin.Sample.Ebay.Components;
    using global::Plugin.Sample.Ebay.Entities;
    using global::Plugin.Sample.Ebay.Models;
    using global::Plugin.Sample.Ebay.Pipelines;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Pricing;

    public class EbayCommand : CommerceCommand
    {
        private readonly IPrepareEbayItemPipeline _pipeline;
        private readonly CommerceCommander _commerceCommander;

        private static ApiContext apiContext;

        public EbayCommand(IPrepareEbayItemPipeline pipeline, CommerceCommander commerceCommander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._pipeline = pipeline;
            this._commerceCommander = commerceCommander;
        }

        public async Task<bool> PublishPending(CommerceContext commerceContext)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                try
                {
                    var ebayPendingSellableItems = await this._commerceCommander.Command<ListCommander>()
                        .GetListItems<SellableItem>(commerceContext, "Ebay_Pending", 0, 10);

                    foreach (var sellableItem in ebayPendingSellableItems)
                    {
                        var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                        var ebayItem = await this._commerceCommander.Command<EbayCommand>().AddItem(commerceContext, sellableItem);
                        ebayItem.ListingDuration = "Days_10";
                        ebayItemComponent.EbayId = ebayItem.ItemID;
                        ebayItemComponent.Status = "Listed";
                        sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");

                        await this._commerceCommander.PersistEntity(commerceContext, sellableItem);

                        await this._commerceCommander.Command<ListCommander>()
                            .RemoveItemsFromList(commerceContext, "Ebay_Pending", new List<string>() { sellableItem.Id });
                    }
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"Ebay.PublishPendingCommand.Exception: Message={ex.Message}");
                }

                return true;
            }
        }
        
        public async Task<ItemType> AddItem(CommerceContext commerceContext, SellableItem sellableItem)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                try
                {
                    // Instantiate the call wrapper class
                    var apiCall = new AddFixedPriceItemCall(await this.GetEbayContext(commerceContext));

                    var item = await this.PrepareItem(commerceContext, sellableItem);

                    // Send the call to eBay and get the results
                    var feeTypeCollection = apiCall.AddFixedPriceItem(item);

                    foreach (var feeItem in feeTypeCollection)
                    {
                        var fee = feeItem as FeeType;
                        ebayItemComponent.Fees.Add(new AwardedAdjustment { Adjustment = new Money(fee.Fee.currencyID.ToString(), System.Convert.ToDecimal(fee.Fee.Value)), AdjustmentType = "Fee", Name = fee.Name });
                    }

                    ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = "Listing Added", EventUser = commerceContext.CurrentCsrId() });
                    ebayItemComponent.EbayId = item.ItemID;
                    ebayItemComponent.Status = "Listed";
                    sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");
                    await commerceContext.AddMessage("Info", "EbayCommand.AddItem", new object[] { item.ItemID }, $"Item Listed:{item.ItemID}");

                    return item;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("It looks like this listing is for an item you already have on eBay"))
                    {
                        var existingId = ex.Message.Substring(ex.Message.IndexOf("(") + 1);
                        existingId = existingId.Substring(0, existingId.IndexOf(")"));
                        await commerceContext.AddMessage("Warn", "EbayCommand.AddItem", new object[] { existingId }, $"ExistingId:{existingId}-ComponentId:{ebayItemComponent.EbayId}");

                        ebayItemComponent.EbayId = existingId;
                        ebayItemComponent.Status = "Listed";
                        ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = "Existing Listing Linked", EventUser = commerceContext.CurrentCsrId() });
                        sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");
                    }
                    else
                    {
                        commerceContext.Logger.LogError($"Ebay.AddItem.Exception: Message={ex.Message}");
                        await commerceContext.AddMessage("Error", "Ebay.AddItem.Exception", new object[] { ex }, ex.Message);

                        ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = $"Error-{ex.Message}", EventUser = commerceContext.CurrentCsrId() });
                    }
                }
                return new ItemType();
            }
        }
        
        public async Task<ItemType> PrepareItem(CommerceContext commerceContext, SellableItem sellableItem)
        {
            ShippingServiceOptionsType shipservice1 = new ShippingServiceOptionsType
            {
                ShippingService = "USPSPriority",
                ShippingServicePriority = 1,
                ShippingServiceCost = new AmountType { currencyID = CurrencyCodeType.USD, Value = 5.0 }
            };
            using (CommandActivity.Start(commerceContext, this))
            {
                var item = await this._commerceCommander.Pipeline<IPrepareEbayItemPipeline>().Run(sellableItem, commerceContext.GetPipelineContextOptions());
                
                item.Description = sellableItem.Description;
                item.Title = sellableItem.DisplayName;
                item.SubTitle = "Test Item";
                
                var listPricingPolicy = sellableItem.GetPolicy<ListPricingPolicy>();
                var listPrice = listPricingPolicy.Prices.FirstOrDefault();

                item.StartPrice = new AmountType { currencyID = CurrencyCodeType.USD, Value = System.Convert.ToDouble(listPrice.Amount) };

                item.ConditionID = 1000;

                item.PaymentMethods = new BuyerPaymentMethodCodeTypeCollection
                {
                    BuyerPaymentMethodCodeType.PayPal, BuyerPaymentMethodCodeType.VisaMC
                };
                item.PayPalEmailAddress = "test@test.com";
                item.PostalCode = "98014";

                item.DispatchTimeMax = 3;
                item.ShippingDetails = new ShippingDetailsType
                {
                    ShippingServiceOptions = new ShippingServiceOptionsTypeCollection(),
                    ShippingType = ShippingTypeCodeType.Flat
                };

                shipservice1.ShippingServiceAdditionalCost = new AmountType
                {
                    currencyID = CurrencyCodeType.USD, Value = 1.0
                };

                item.ShippingDetails.ShippingServiceOptions.Add(shipservice1);

                item.ReturnPolicy = new ReturnPolicyType { ReturnsAcceptedOption = "ReturnsAccepted" };

                // Add pictures
                item.PictureDetails = new PictureDetailsType
                {
                    GalleryType = GalleryTypeCodeType.None, GalleryTypeSpecified = true
                };
                
                return item;
            }
        }
        
        public async Task<bool> EndItemListing(CommerceContext commerceContext, SellableItem sellableItem, string reason)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                try
                {
                    var apiCall = new eBay.Service.Call.EndItemCall(await this.GetEbayContext(commerceContext));

                    if (sellableItem.HasComponent<EbayItemComponent>())
                    {
                        var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                        var reasonCodeType = EndReasonCodeType.NotAvailable;

                        switch (reason)
                        {
                            case "NotAvailable":
                                reasonCodeType = EndReasonCodeType.NotAvailable;
                                break;
                            case "CustomCode":
                                reasonCodeType = EndReasonCodeType.CustomCode;
                                break;
                            case "Incorrect":
                                reasonCodeType = EndReasonCodeType.Incorrect;
                                break;
                            case "LostOrBroken":
                                reasonCodeType = EndReasonCodeType.LostOrBroken;
                                break;
                            case "OtherListingError":
                                reasonCodeType = EndReasonCodeType.OtherListingError;
                                break;
                            case "SellToHighBidder":
                                reasonCodeType = EndReasonCodeType.SellToHighBidder;
                                break;
                            case "Sold":
                                reasonCodeType = EndReasonCodeType.Sold;
                                break;
                            default:
                                reasonCodeType = EndReasonCodeType.CustomCode;
                                break;
                        }

                        if (string.IsNullOrEmpty(ebayItemComponent.EbayId))
                        {
                            ebayItemComponent.Status = "LostSync";
                        }
                        else
                        {
                            if (ebayItemComponent.Status != "Ended")
                            {
                                // Call Ebay and End the Item Listing
                                try
                                {
                                    apiCall.EndItem(ebayItemComponent.EbayId, reasonCodeType);
                                    ebayItemComponent.Status = "Ended";
                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message == "The auction has already been closed.")
                                    {
                                        // Capture a case where the listing has expired naturally and it can now no longer be ended.
                                        reason = "Expired";
                                        ebayItemComponent.Status = "Ended";
                                    }
                                    else
                                    {
                                        commerceContext.Logger.LogError(ex, $"EbayCommand.EndItemListing.Exception: Message={ex.Message}");
                                        await commerceContext.AddMessage("Error", "EbayCommand.EndItemListing", new object[] { ex }, ex.Message);
                                    }
                                }
                            }
                        }

                        ebayItemComponent.ReasonEnded = reason;

                        ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = "Listing Ended", EventUser = commerceContext.CurrentCsrId() });

                        sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Ended");

                        await this._commerceCommander.PersistEntity(commerceContext, sellableItem);

                        await this._commerceCommander.Command<ListCommander>()
                            .RemoveItemsFromList(commerceContext, "Ebay_Listed", new List<string>() { sellableItem.Id });
                    }
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"Ebay.EndItemListing.Exception: Message={ex.Message}");
                    await commerceContext.AddMessage("Error", "Ebay.EndItemListing.Exception", new object[] { ex }, ex.Message);
                }

                return true;
            }
        }

        public async Task<ItemType> RelistItem(CommerceContext commerceContext, SellableItem sellableItem)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var apiCall = new eBay.Service.Call.RelistItemCall(await this.GetEbayContext(commerceContext));

                if (sellableItem.HasComponent<EbayItemComponent>())
                {
                    var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                    try
                    {
                        var item = await this.PrepareItem(commerceContext, sellableItem);
                        item.ItemID = ebayItemComponent.EbayId;
                        
                        // Send the call to eBay and get the results
                        var feeResult = apiCall.RelistItem(item, new StringCollection());

                        ebayItemComponent.EbayId = item.ItemID;

                        ebayItemComponent.Status = "Listed";
                        sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");

                        foreach (var feeItem in feeResult)
                        {
                            var fee = feeItem as FeeType;
                            ebayItemComponent.Fees.Add(new AwardedAdjustment { Adjustment = new Money(fee.Fee.currencyID.ToString(), System.Convert.ToDecimal(fee.Fee.Value)), AdjustmentType = "Fee", Name = fee.Name });
                        }

                        ebayItemComponent.History.Add(new HistoryEntryModel { EventMessage = "Listing Relisted", EventUser = commerceContext.CurrentCsrId() });

                        return item;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("It looks like this listing is for an item you already have on eBay"))
                        {
                            var existingId = ex.Message.Substring(ex.Message.IndexOf("(") + 1);
                            existingId = existingId.Substring(0, existingId.IndexOf(")"));
                            await commerceContext.AddMessage("Warn", "Ebay.RelistItem", new object[] { }, $"ExistingId:{existingId}-ComponentId:{ebayItemComponent.EbayId}");
                            ebayItemComponent.EbayId = existingId;
                            ebayItemComponent.Status = "Listed";
                            sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");
                        }
                        else
                        {
                            commerceContext.Logger.LogError($"Ebay.RelistItem.Exception: Message={ex.Message}");
                            await commerceContext.AddMessage("Error", "Ebay.RelistItem.Exception", new object[] { ex }, ex.Message);
                        }
                    }
                }
                else
                {
                    commerceContext.Logger.LogError("EbayCommand.RelistItem.Exception: Message=ebayCommand.RelistItem.NoEbayItemComponent");
                    await commerceContext.AddMessage("Error", "Ebay.RelistItem.Exception", new object[] { }, "ebayCommand.RelistItem.NoEbayItemComponent");
                }

                return new ItemType();
            }
        }

        public async Task<ItemType> GetItem(CommerceContext commerceContext, string itemId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                // Instantiate the call wrapper class
                var apiCall = new GetItemCall(await this.GetEbayContext(commerceContext));

                try
                {
                    var result = apiCall.GetItem(itemId);
                    return result;
                }
                catch (Exception ex)
                {
                    // commerceContext.LogException("GetItem", ex);
                    await commerceContext.AddMessage("Warn", "Ebay.GetItem.Exception", new object[] { ex }, ex.Message);
                }

                return new ItemType();
            }
        }

        public async Task<DateTime> GetEbayTime(CommerceContext commerceContext)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                // Instantiate the call wrapper class
                var apiCall = new GeteBayOfficialTimeCall(await this.GetEbayContext(commerceContext));

                // Send the call to eBay and get the results
                try
                {
                    var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(commerceContext, "Entity-EbayConfigEntity-Global", true);

                    if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                    {
                        var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                        if (!string.IsNullOrEmpty(ebayConfigComponent.EbayToken))
                        {
                            var officialTime = apiCall.GeteBayOfficialTime();
                            return officialTime;
                        }
                    }
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"Ebay.GetEbayTime.Exception: Message={ex.Message}");
                    await commerceContext.AddMessage("Error", "Ebay.GetEbayTime.Exception", new object[] { ex }, ex.Message);
                }

                return new DateTime();
            }
        }

        public async Task<SuggestedCategoryTypeCollection> GetSuggestedCategories(CommerceContext commerceContext, string query)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                try
                {
                    var getSuggestedCategories = new eBay.Service.Call.GetSuggestedCategoriesCall(await this.GetEbayContext(commerceContext));
                    var result = getSuggestedCategories.GetSuggestedCategories(query);

                    return result;
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"Ebay.GetCategories.Exception: Message={ex.Message}");
                }
                return new SuggestedCategoryTypeCollection();
            }
        }

        public async Task<ApiContext> GetEbayContext(CommerceContext commerceContext)
        {
            // apiContext is a singleton,
            if (apiContext != null)
            {
                return apiContext;
            }

            apiContext = new ApiContext();
            var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(commerceContext, "Entity-EbayConfigEntity-Global", true);

            if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
            {
                var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();

                //Supply user token
                ApiCredential apiCredential = new ApiCredential();
                apiContext.ApiCredential = apiCredential;

                if (!string.IsNullOrEmpty(ebayConfigComponent.EbayToken))
                {
                    apiCredential.eBayToken = ebayConfigComponent.EbayToken;
                }
            }

            // supply Api Server Url
            apiContext.SoapApiServerUrl = "https://api.sandbox.ebay.com/wsapi";

            // Specify site: here we use US site
            apiContext.Site = SiteCodeType.US;

            return apiContext;
        }
    }
}