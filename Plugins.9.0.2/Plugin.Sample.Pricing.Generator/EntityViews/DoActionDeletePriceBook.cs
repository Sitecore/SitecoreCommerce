
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
    [PipelineDisplayName("DoActionDeletePriceBook")]
    public class DoActionDeletePriceBook : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionDeletePriceBook"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionDeletePriceBook(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("Pricing-DeletePriceBook", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var bookName = entityView.ItemId.Replace("Entity-PriceBook-", "");

                await this._commerceCommander.Command<ClearPriceCards>()
                    .Process(context.CommerceContext, entityView.ItemId);

                var deleteBookResult = await this._commerceCommander.DeleteEntity(context.CommerceContext, $"Entity-PriceBook-{bookName}");
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Pricing.DoActionDeletePriceBook.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
