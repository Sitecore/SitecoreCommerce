namespace Plugin.Sample.BizFx.Enhancements.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("EnsurePriceCardSnapshots")]
    public class EnsurePriceCardSnapshots : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.EntityId.Contains("Entity-PriceCard-"))
            {
                var priceCardSnapshotsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "PriceCardSnapshots");
                if (priceCardSnapshotsView != null)
                {
                    foreach(var itemRow in (priceCardSnapshotsView as EntityView).ChildViews)
                    {
                        var priceCardFoundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == (itemRow as EntityView).ItemId);
                        
                        if (priceCardFoundEntity != null)
                        {
                            if (priceCardFoundEntity.Entity is PriceCard priceCard)
                            {
                                var imagesComponent = priceCard.GetComponent<ImagesComponent>();
                                var firstImage = imagesComponent.Images.FirstOrDefault();
                                firstImage = firstImage.Replace("-", "");

                                if (firstImage != null)
                                {
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
