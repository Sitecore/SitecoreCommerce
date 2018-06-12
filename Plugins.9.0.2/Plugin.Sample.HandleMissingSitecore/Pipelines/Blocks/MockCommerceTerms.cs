// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockCommerceTerms.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.HandleMissingSitecore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Management;
    using Sitecore.Commerce.Plugin.Shops;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the validate storefront currency block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.Boolean, System.Boolean,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("MockCommerceTerms")]
    public class MockCommerceTerms : PipelineBlock<CommerceTermsArgument, List<Sitecore.Commerce.Core.LocalizedTerm>, CommercePipelineExecutionContext>
    {
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
        /// True if the Storefront's currency is valid, false otherwise.
        /// </returns>
        public override Task<List<Sitecore.Commerce.Core.LocalizedTerm>> Run(CommerceTermsArgument arg, CommercePipelineExecutionContext context)
        {

            return Task.FromResult(new List<LocalizedTerm>(0));

        }
    }
}
