// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionClearAppServices.cs" company="Sitecore Corporation">
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
    [PipelineDisplayName("DoActionClearAppServices")]
    public class DoActionClearAppServices : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionClearAppServices"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionClearAppServices(
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
                || !entityView.Action.Equals("DevOps-ClearAppServices", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }


            var appServices = await this._commerceCommander.Command<ListCommander>().GetListItems<AppService>(context.CommerceContext,"AppServices", 0, 999);

            foreach(var appService in appServices)
            {
                var deleteResult = await this._commerceCommander.Command<DeleteEntityCommand>().Process(context.CommerceContext, appService.Id);
            }

            return entityView;
        }
    }
}
