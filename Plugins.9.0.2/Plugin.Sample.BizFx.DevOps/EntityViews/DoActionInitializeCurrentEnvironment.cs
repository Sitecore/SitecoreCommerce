// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionInitializeCurrentEnvironment.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;

    using Sitecore.Commerce.EntityViews;
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
    [PipelineDisplayName("DoActionInitializeCurrentEnvironment")]
    public class DoActionInitializeCurrentEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionInitializeCurrentEnvironment"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionInitializeCurrentEnvironment(
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
                || !entityView.Action.Equals("DevOps-InitializeCurrentEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var appService = await this._commerceCommander.GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

            try
            {
                var initializeEnvironmentResponse = await this._commerceCommander.Command<InitializeEnvironmentCommand>()
                    .Process(context.CommerceContext, context.CommerceContext.Environment.Name);

                //var jsonResponse = await this._commerceCommander.Command<JsonCommander>()
                //    .Process(context.CommerceContext, $"http://{appService.Host}/commerceops/InitializeEnvironment(environment='{context.CommerceContext.Environment.Name}')");

                //appService.GetComponent<ActionHistoryComponent>()
                //    .AddHistory(new ActionHistoryModel
                //    {
                //        Name = entityView.Action,
                //        Response = "Ok",
                //        JSON = jsonResponse.Json,
                //        EntityId = entityView.EntityId,
                //        ItemId = entityView.ItemId
                //    });


                //var result = await this._commerceCommander.PersistEntity(context.CommerceContext, appService);

                //appService.GetComponent<ActionHistoryComponent>()
                //    .AddHistory(new ActionHistoryModel
                //    {
                //        Name = entityView.Action,
                //        Response = "Ok",
                //        JSON = jsonResponse.Json,
                //        EntityId = entityView.EntityId,
                //        ItemId = entityView.ItemId
                //    });

            }
            catch(Exception ex)
            {
                context.CommerceContext.LogException("DoActionInitializeCurrentEnvironment", ex);
                //appService.GetComponent<ActionHistoryComponent>()
                //    .AddHistory(new ActionHistoryModel
                //    {
                //        Name = entityView.Action,
                //        Response = "Exception",
                //        JSON = ex.StackTrace,
                //        EntityId = entityView.EntityId,
                //        ItemId = entityView.ItemId
                //    });
            }

            

            return entityView;
        }

   

   

                


   }
}
