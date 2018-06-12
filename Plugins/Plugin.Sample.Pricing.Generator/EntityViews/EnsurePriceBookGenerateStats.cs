
using System.Threading.Tasks;

namespace Plugin.Sample.Pricing.Generator
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Linq;

    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsurePriceBookGenerateStats")]
    public class EnsurePriceBookGenerateStats : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormGenerateSamplePriceBook"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsurePriceBookGenerateStats(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "PricingDashboard")
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var priceBooksView = entityView.ChildViews.FirstOrDefault(p => p.Name == "PriceBooks");
            if (priceBooksView != null)
            {
                foreach(EntityView childView in (priceBooksView as EntityView).ChildViews)
                {
                    var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == childView.ItemId);
                    if (foundEntity != null)
                    {
                        PriceBook priceBook = foundEntity.Entity as PriceBook;

                        var listName = string.Format(context.GetPolicy<KnownPricingListsPolicy>().PriceBookCards, priceBook.Name);
                        var listCount = await this._commerceCommander.Command<GetListCountCommand>()
                            .Process(context.CommerceContext, listName);
                        childView.Properties.Add(new ViewProperty { Name = "Count", DisplayName = "Count", RawValue = listCount });

                        if (priceBook.HasComponent<ActionHistoryComponent>())
                        {
                            var actionHistoryComponent = priceBook.GetComponent<ActionHistoryComponent>();
                            var actionHistoryModel = actionHistoryComponent.History.First();

                            childView.Properties.Add(new ViewProperty { Name = "Start", DisplayName = "Started", RawValue = actionHistoryModel.StartTime.ToString("yyyy-MMM-dd hh:mm:ss") });
                            childView.Properties.Add(new ViewProperty { Name = "Complete", DisplayName = "Completed", RawValue = actionHistoryModel.Completed.ToString("yyyy-MMM-dd hh:mm:ss") });

                            var elapsed = (actionHistoryModel.Completed - actionHistoryModel.StartTime).TotalMilliseconds;

                            childView.Properties.Add(new ViewProperty { Name = "Elapse", DisplayName = "Elapsed", RawValue = System.Convert.ToInt32(System.Math.Round(elapsed, 0)) });
                        }
                        else
                        {
                            childView.Properties.Add(new ViewProperty { Name = "Start", DisplayName = "Started", RawValue = "" });
                            childView.Properties.Add(new ViewProperty { Name = "Complete", DisplayName = "Completed", RawValue = "" });
                            childView.Properties.Add(new ViewProperty { Name = "Elapse", DisplayName = "Elapsed", RawValue = "" });
                        }
                    }
                }
            }


            return entityView;
        }


    }

}
