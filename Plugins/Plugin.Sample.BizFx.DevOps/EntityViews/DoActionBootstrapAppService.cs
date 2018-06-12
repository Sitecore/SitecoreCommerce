// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionBootstrapAppService.cs" company="Sitecore Corporation">
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
    using Sitecore.Commerce.Core.Commands;

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionBootstrapAppService")]
    public class DoActionBootstrapAppService : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionBootstrapAppService"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">The persistEntityPipeline.</param>
        /// <param name="findEntityPipeline">The findEntityPipeline.</param>>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionBootstrapAppService(
            IPersistEntityPipeline persistEntityPipeline, 
            IFindEntityPipeline findEntityPipeline,
            CommerceCommander commerceCommander)
        {
            this._persistEntityPipeline = persistEntityPipeline;
            this._findEntityPipeline = findEntityPipeline;
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
                || !entityView.Action.Equals("DevOps-BootStrapAppService", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            //var appService = (await this._findEntityPipeline.Run(new FindEntityArgument(typeof(CommerceEntity), entityView.ItemId, false), context.CommerceContext.GetPipelineContextOptions())) as AppService;

            var serviceResponse = await this._commerceCommander.Command<BootstrapCommand>().Process(context.CommerceContext);

            //var serviceResponse = await this._commerceCommander.Command<JsonCommander>().Post(context.CommerceContext, $"http://{appService.Host}/commerceops/Bootstrap()", "");

            //appService.GetComponent<ActionHistoryComponent>()
            //    .AddHistory(new ActionHistoryModel {
            //    Name = entityView.Action, Response = "Ok",
            //        JSON = serviceResponse.Json,
            //        EntityId = entityView.EntityId,
            //        ItemId = entityView.ItemId
            //    } );


            //var result = await this._persistEntityPipeline.Run(new PersistEntityArgument(appService), context);

            return entityView;
        }

   

   

                


   }
}
