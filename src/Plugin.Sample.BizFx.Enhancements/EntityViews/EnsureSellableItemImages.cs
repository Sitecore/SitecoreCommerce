namespace Plugin.Sample.BizFx.Enhancements.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("EnsureTaggedViews")]
    public class EnsureSellableItemImages : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.EntityId.Contains("Entity-Category-"))
            {
                var sellableItemsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "SellableItems");
                if (sellableItemsView != null)
                {
                    foreach(var sellableItemRow in (sellableItemsView as EntityView).ChildViews)
                    {
                        var sellableItemFoundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == (sellableItemRow as EntityView).ItemId);

                        if (sellableItemFoundEntity != null)
                        {
                            if (sellableItemFoundEntity.Entity is SellableItem sellableItem)
                            {
                                var imagesComponent = sellableItem.GetComponent<ImagesComponent>();
                                var firstImage = imagesComponent.Images.FirstOrDefault();
                                firstImage = firstImage.Replace("-", "");

                                if (firstImage != null)
                                {
                                    (sellableItemRow as EntityView).Properties.Insert(1, new ViewProperty
                                    {
                                        Name = " ",
                                        IsHidden = false,
                                        IsReadOnly = true,
                                        OriginalType = "Html",
                                        UiType = "Html",
                                        IsRequired = false,
                                        Value = $"<img alt='' height=50 width=50 src='https://sxa.storefront.com/-/media/{firstImage}.ashx'/>",
                                        RawValue = "<img alt='' height=50 width=50 src='https://sxa.storefront.com/-/media/372d8bc66888437591c1f3bee2d31558.ashx'/>"
                                    });
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
