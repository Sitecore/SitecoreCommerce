// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionAddAppService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Management;
    using Microsoft.Extensions.Logging;
    using Sitecore.Services.Core.Model;
    using Sitecore.Commerce.Plugin.ManagedLists;

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionAddAppService")]
    public class DoActionAddAppService : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionAddAppService"/> class.
        /// </summary>
        /// <param name="findEntityPipeline">The findEntityPipeline.</param>
        public DoActionAddAppService( 
            IFindEntityPipeline findEntityPipeline
            )
        {
            this._findEntityPipeline = findEntityPipeline;
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
                || !entityView.Action.Equals("DevOps-AddAppService", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }


            var findResult = await this._findEntityPipeline.Run(new FindEntityArgument(typeof(CommerceEntity), entityView.ItemId, false), context.CommerceContext.GetPipelineContextOptions());


            return entityView;
        }

   

   

                


   }
}
