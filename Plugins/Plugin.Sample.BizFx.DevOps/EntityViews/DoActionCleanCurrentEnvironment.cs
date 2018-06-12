// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionCleanCurrentEnvironment.cs" company="Sitecore Corporation">
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
    using Plugin.Sample.JsonCommander;

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionCleanCurrentEnvironment")]
    public class DoActionCleanCurrentEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionCleanCurrentEnvironment"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionCleanCurrentEnvironment(
            CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("DevOps-CleanCurrentEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            //var appService = await this._commerceCommander.GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

            //this._commerceCommander.Command<FindE>
            //var appService = (await this._commerceCommander.Pipeline<FindEntityPipeline>()
            //    .Run(new FindEntityArgument(typeof(CommerceEntity),
            //    entityView.ItemId, false), context.CommerceContext.GetPipelineContextOptions())) as AppService;

            //var bodyJson = @"{""environment"":""AdventureWorksAuthoring""}";

            var jsonAction = new JsonAction { environment = context.CommerceContext.Environment.Name };

            var cleanEnvironmentResponse = await this._commerceCommander.Command<CleanEnvironmentCommand>()
                .Process(context.CommerceContext.Environment.Name, context.CommerceContext);

            //var jsonResponse = await this._commerceCommander.Command<JsonCommander>()
            //    .Put(context.CommerceContext, $"http://{appService.Host}/commerceops/CleanEnvironment()", jsonAction);

            //appService.GetComponent<ActionHistoryComponent>()
            //    .AddHistory(new ActionHistoryModel
            //    {
            //        Name = entityView.Action,
            //        Response = "Ok",
            //        JSON = jsonResponse.Json,
            //        EntityId = entityView.EntityId,
            //        ItemId = entityView.ItemId
            //    });

            //var result = await this._persistEntityPipeline.Run(new PersistEntityArgument(appService), context);

            return entityView;
        }

   

   

                


   }
}
