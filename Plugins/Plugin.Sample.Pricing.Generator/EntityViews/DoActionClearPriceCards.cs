
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
    using System.Threading;

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionClearPriceCards")]
    public class DoActionClearPriceCards : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionClearPriceCards"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionClearPriceCards(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("Pricing-ClearPriceCards", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                await this._commerceCommander.Command<ClearPriceCards>()
                    .Process(context.CommerceContext, entityView.ItemId);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Pricing.DoActionGeneratorSamplePriceBook.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
