// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessListMembershipsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.PerformanceTuning
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the process list memberships block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.PersistEntityArgument,
    ///         Sitecore.Commerce.Core.PersistEntityArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ManagedListsConstants.Pipelines.Blocks.ProcessListMemberships)]
    public class ProcessListMembershipsBlock : PipelineBlock<PersistEntityArgument, PersistEntityArgument, CommercePipelineExecutionContext>
    {
        private readonly IAddListEntitiesPipeline _addListEntitiesPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessListMembershipsBlock"/> class.
        /// </summary>
        /// <param name="addListEntitiesPipeline">The add list entities pipeline.</param>
        public ProcessListMembershipsBlock(IAddListEntitiesPipeline addListEntitiesPipeline)
        {
            this._addListEntitiesPipeline = addListEntitiesPipeline;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="PersistEntityArgument"/></returns>
        public override async Task<PersistEntityArgument> Run(PersistEntityArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The block argument can not be null.");

            if (!arg.Entity.HasComponent<ListMembershipsComponent>())
            {
                return arg;
            }

            if (!arg.Entity.IsPersisted)
            {
                // PROCESS LIST MEMBERSHIPS
                var lists = arg.Entity.GetComponent<ListMembershipsComponent>();
                foreach (var list in lists.Memberships)
                {
                    await this._addListEntitiesPipeline.Run(new ListEntitiesArgument(new List<string> { arg.Entity.Id }, list), context);
                }
            }

            return arg;
        }
    }
}
