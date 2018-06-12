// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterPriceSnapshotsByTagsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.PerformanceTuning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the filter price snapshots by tags block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.Collections.Generic.IEnumerable{Sitecore.Commerce.Plugin.Pricing.PriceCard},
    ///         Sitecore.Commerce.Plugin.Pricing.PriceSnapshotComponent, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(PricingConstants.Pipelines.Blocks.FilterPriceSnapshotsByTagsBlock)]
    public class FilterPriceSnapshotsByTagsBlock : PipelineBlock<IEnumerable<PriceCard>, PriceSnapshotComponent, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A <see cref="PriceSnapshotComponent" />
        /// </returns>
        public override Task<PriceSnapshotComponent> Run(IEnumerable<PriceCard> arg, CommercePipelineExecutionContext context)
        {
            if (arg != null)
            {
                // Clone price cards to prevent modification to the original entity which might be cached.
                //var cards = arg.Select(x => x.Clone<PriceCard>()).Cast<PriceCard>().ToList();
                //var cards = arg.ToList();

                if (arg.Any())
                {
                    var tags = context.CommerceContext.GetObject<IEnumerable<Tag>>()?.ToList();
                    if (tags != null && tags.Any())
                    {
                        var effectiveDate = context.CommerceContext.CurrentEffectiveDate();
                        var allSnapshots = new List<PriceSnapshotComponent>();
                        arg.ForEach(card => allSnapshots.AddRange(card.Snapshots.Where(s => s.Tags.Any())));

                        var approvedSnapshots = allSnapshots.Where(s => s.IsApproved(context.CommerceContext) && s.BeginDate.CompareTo(effectiveDate) <= 0);
                        var filteredSnapshotsByTags = approvedSnapshots.Where(s => tags.Select(t => t.Name).Intersect(s.Tags.Select(t => t.Name), StringComparer.OrdinalIgnoreCase).Any());
                        var filteredSnapshotsByTagsOrdered = filteredSnapshotsByTags.OrderByDescending(s => s.Tags.Select(t => t.Name).Intersect(tags.Select(t => t.Name), StringComparer.OrdinalIgnoreCase).Count());
                        var snapshot = filteredSnapshotsByTagsOrdered.ThenByDescending(s => s.BeginDate).FirstOrDefault();
                        if (snapshot != null)
                        {
                            var currency = context.CommerceContext.CurrentCurrency();
                            var tiers = snapshot.Tiers.ToList();
                            tiers.RemoveAll(t => !t.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase));
                            snapshot.Tiers = tiers;
                            return Task.FromResult(snapshot);
                        }
                    }
                }
            }
            
            context.Logger.LogDebug($"{this.Name}.ActivePriceSnapshot.NotFound: EffectiveDate={context.CommerceContext.CurrentEffectiveDate().ToString()}");
            return Task.FromResult((PriceSnapshotComponent)null);
        }
    }
}