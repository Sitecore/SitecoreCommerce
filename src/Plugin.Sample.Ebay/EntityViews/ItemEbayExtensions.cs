namespace Plugin.Sample.Ebay.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Commands;
    using global::Plugin.Sample.Ebay.Components;
    using global::Plugin.Sample.Ebay.Entities;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("Sitecore.Commerce.Plugin.Ebay")]
    public class ItemEbayExtensions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public ItemEbayExtensions(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

            try
            {
                if (entityView.Name != "Master")
                {
                    return entityView;
                }
                if (!entityView.EntityId.Contains("Entity-SellableItem-"))
                {
                    return entityView;
                }

                var ebayConfig = await _commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

                if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                {
                    var entityViewArgument = _commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

                    if (!(entityViewArgument.Entity is SellableItem))
                    {
                        return entityView;
                    }

                    var item = (SellableItem)entityViewArgument.Entity;
                    if (item.HasComponent<EbayItemComponent>())
                    {
                        var ebayItemComponent = item.GetComponent<EbayItemComponent>();

                        var childView = new EntityView
                        {
                            Name = "Ebay Marketplace Item",
                            UiHint = "Flat",
                            Icon = "market_stand",
                            DisplayRank = 200,
                            EntityId = item.Id,
                            ItemId = string.Empty
                        };
                        entityView.ChildViews.Add(childView);

                        childView.Properties.Add(
                            new ViewProperty
                            {
                                Name = string.Empty,
                                IsHidden = false,
                                IsReadOnly = true,
                                OriginalType = "Html",
                                UiType = "Html",
                                RawValue = "<img alt='This is the alternate' height=50 width=100 src='https://www.paypalobjects.com/webstatic/en_AR/mktg/merchant/pages/sell-on-ebay/image-ebay.png' style=''/>"
                            });

                        childView.Properties.Add(
                            new ViewProperty
                            {
                                Name = "EbayId",
                                IsHidden = false,
                                IsReadOnly = true,
                                UiType = "Html",
                                OriginalType = "Html",
                                RawValue = $"<a href='http://cgi.sandbox.ebay.com/ws/eBayISAPI.dll?ViewItem&item={ebayItemComponent.EbayId}&ssPageName=STRK:MESELX:IT&_trksid=p3984.m1558.l2649#ht_500wt_1157' target='_blank'>{ebayItemComponent.EbayId}</a> "
                            });

                        childView.Properties.Add(
                            new ViewProperty
                            {
                                Name = "Status",
                                IsHidden = false,
                                IsReadOnly = true,
                                RawValue = ebayItemComponent.Status
                            });

                        if (ebayItemComponent.Status == "Ended")
                        {
                            childView.Properties.Add(
                                new ViewProperty
                                {
                                    Name = "ReasonEnded",
                                    DisplayName = "Reason Ended",
                                    IsHidden = false,
                                    IsReadOnly = true,
                                    RawValue = ebayItemComponent.ReasonEnded
                                });
                        }

                        var history = "========================================<BR>";
                        foreach (var historyItem in ebayItemComponent.History)
                        {
                            history = history + $"{historyItem.EventDate.ToString("yyyy-MMM-dd hh:mm")}-{historyItem.EventMessage}-{historyItem.EventUser.Replace(@"sitecore\", "")}<BR>";
                        }

                        childView.Properties.Add(
                            new ViewProperty
                            {
                                Name = "History",
                                IsHidden = false,
                                IsReadOnly = true,
                                UiType = "Html",
                                OriginalType = "Html",
                                RawValue = history
                            });

                        if (ebayItemComponent.Status == "Listed")
                        {
                            if (!string.IsNullOrEmpty(ebayItemComponent.EbayId))
                            {
                                var ebayItem = await _commerceCommander.Command<EbayCommand>().GetItem(context.CommerceContext, ebayItemComponent.EbayId);
                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "EndTime",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.ListingDetails.EndTime
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Sku",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.SKU
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Price",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = new Money("USD", System.Convert.ToDecimal(ebayItem.StartPrice.Value))
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Country",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.Country.ToString()
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Currency",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.Currency.ToString()
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Location",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.Location
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "ListingDuration",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.ListingDuration
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "Quantity",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.Quantity
                                    });

                                childView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = "PrimaryCategory",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        RawValue = ebayItem.PrimaryCategory.CategoryID
                                    });

                                var ebayItemFeesView = new EntityView
                                {
                                    Name = "Ebay Item Fees",
                                    UiHint = "Flat",
                                    Icon = "market_stand",
                                    DisplayRank = 200,
                                    EntityId = item.Id,
                                    ItemId = string.Empty
                                };
                                entityView.ChildViews.Add(ebayItemFeesView);

                                foreach (var fee in ebayItemComponent.Fees)
                                {
                                    ebayItemFeesView.Properties.Add(
                                        new ViewProperty
                                        {
                                            Name = fee.Name,
                                            IsHidden = false,
                                            IsReadOnly = true,
                                            RawValue = fee.Adjustment
                                        });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.DoActionEndItem.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "ItemEbayExtensions.Run.Exception", new object[] { ex }, ex.Message);
            }

            return entityView;
        }
    }
}
