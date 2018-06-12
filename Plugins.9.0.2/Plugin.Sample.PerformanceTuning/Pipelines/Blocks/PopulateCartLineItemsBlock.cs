// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateCartLineItemsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.PerformanceTuning
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which iterates through a Cart and runs the PopulateCartLineItemPipeline for each line in the cart.
    /// </summary>
    [PipelineDisplayName(CartsConstants.Pipelines.Blocks.PopulateCartLineItemsBlock)]
    public class PopulateCartLineItemsBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private readonly IPopulateLineItemPipeline _populateLineItemPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopulateCartLineItemsBlock" /> class.
        /// </summary>
        /// <param name="populateLineItemPipeline">The get sellable item pipeline.</param>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        public PopulateCartLineItemsBlock(IPopulateLineItemPipeline populateLineItemPipeline, IPersistEntityPipeline persistEntityPipeline)
        {
            this._populateLineItemPipeline = populateLineItemPipeline;
            this._persistEntityPipeline = persistEntityPipeline;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Cart"/>.
        /// </returns>
        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The cart can not be null");
            Condition.Requires(arg.Lines).IsNotNull($"{this.Name} :The cart's lines can not be null");

            if (!arg.Lines.Any())
            {
                return arg;
            }
            
            var isDirty = false;
            foreach (var line in arg.Lines.Where(l => l != null).ToList())
            {
                if (string.IsNullOrEmpty(line.ItemId) || line.ItemId.Split('|').Length != 3)
                {
                    continue;
                }

                if (line.Policies.Count == 0)
                {
                    // This pipeline provides the opportunity to process each line, adding descriptive details, inventory availability, etc
                    await this._populateLineItemPipeline.Run(line, context).ConfigureAwait(false);
                }

                if (!context.CommerceContext.GetMessages()
                    .Any(m => !string.IsNullOrEmpty(m.CommerceTermKey) &&
                              m.CommerceTermKey.Equals("EntityNotFound", StringComparison.OrdinalIgnoreCase) &&
                              !string.IsNullOrEmpty(m.Text) && m.Text.IndexOf(line.ItemId) > 0))
                {
                    continue;
                }

                // product not found
                arg.Lines.Remove(line);
                isDirty = true;
            }

            if (isDirty)
            {
                arg = (await this._persistEntityPipeline.Run(new PersistEntityArgument(arg), context)).Entity as Cart;
            }

            return arg;
        }
    }
}
