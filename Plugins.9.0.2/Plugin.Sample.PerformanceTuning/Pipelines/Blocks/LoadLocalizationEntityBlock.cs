// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadLocalizationEntityBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.PerformanceTuning
{
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the load localization entity block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.CommerceEntity,
    ///         Sitecore.Commerce.Core.CommerceEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(CoreConstants.Pipelines.Blocks.LoadLocalizationEntityBlock)]
    public class LoadLocalizationEntityBlock : PipelineBlock<CommerceEntity, CommerceEntity, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadLocalizationEntityBlock"/> class.
        /// </summary>
        /// <param name="findEntityPipeline">The find entity pipeline.</param>
        public LoadLocalizationEntityBlock(IFindEntityPipeline findEntityPipeline)
        {
            this._findPipeline = findEntityPipeline;
        }

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="CommerceEntity"/></returns>
        public override async Task<CommerceEntity> Run(CommerceEntity arg, CommercePipelineExecutionContext context)
        {
            if (arg == null 
                || !arg.HasComponent<LocalizedEntityComponent>()
                || arg is LocalizationEntity)
            {
                return arg;
            }

            var entityId = arg.GetComponent<LocalizedEntityComponent>().Entity.EntityTarget;
            var localizationEntity =
                await this._findPipeline.Run(new FindEntityArgument(typeof(LocalizationEntity), entityId), context);
            if (localizationEntity != null)
            {
                context.CommerceContext.AddEntity(localizationEntity);
            }

            return arg;
        }
    }
}
