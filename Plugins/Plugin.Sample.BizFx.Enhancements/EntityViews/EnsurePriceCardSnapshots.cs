
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.BizFx.Enhancements
{
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Views;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Pricing;

    //using Sitecore.Commerce.Plugin.Catalog;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsurePriceCardSnapshots")]
    public class EnsurePriceCardSnapshots : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsurePriceCardSnapshots"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsurePriceCardSnapshots(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityView.EntityId.Contains("Entity-PriceCard-"))
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);
                //r sellableItem = entityViewArgument.Entity as SellableItem;
                
                var priceCardSnapshotsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "PriceCardSnapshots");
                if (priceCardSnapshotsView != null)
                {
                    foreach(var itemRow in (priceCardSnapshotsView as EntityView).ChildViews)
                    {

                        var priceCardFoundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == (itemRow as EntityView).ItemId);
                        

                        if (priceCardFoundEntity != null)
                        {
                            var priceCard = priceCardFoundEntity.Entity as PriceCard;

                            if (priceCard != null)
                            {
                                var imagesComponent = priceCard.GetComponent<ImagesComponent>();
                                var firstImage = imagesComponent.Images.FirstOrDefault();
                                firstImage = firstImage.Replace("-", "");

                                if (firstImage != null)
                                {
                                    //(sellableItemRow as EntityView).Properties.Insert(1, new ViewProperty
                                    //{
                                    //    Name = " ",
                                    //    IsHidden = false,
                                    //    IsReadOnly = true,
                                    //    OriginalType = "Html",
                                    //    UiType = "Html",
                                    //    IsRequired = false,
                                    //    Value = $"<img alt='' height=50 width=50 src='https://sxa.storefront.com/-/media/{firstImage}.ashx'/>",
                                    //    RawValue = "<img alt='' height=50 width=50 src='https://sxa.storefront.com/-/media/372d8bc66888437591c1f3bee2d31558.ashx'/>"
                                    //});

                                    (itemRow as EntityView).Properties[0] = new ViewProperty
                                    {
                                        Name = " ",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        OriginalType = "Html",
                                        UiType = "Html",
                                        IsRequired = false,
                                        Value = $"<img alt='' height=50 width=50 src='https://sxa.storefront.com/-/media/{firstImage}.ashx'/>",
                                        RawValue = "<img alt='' height=50 width=50 src='https://sxa.storefront.com/-/media/372d8bc66888437591c1f3bee2d31558.ashx'/>"
                                    };

                                }
                            }
                        }


                    }
                }
            }
            return Task.FromResult(entityView);
        }
    }
}
