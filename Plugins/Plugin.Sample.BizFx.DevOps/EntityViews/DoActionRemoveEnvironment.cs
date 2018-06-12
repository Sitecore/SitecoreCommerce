// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionRemoveEnvironment.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    using Microsoft.Extensions.Logging;


    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Management;
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
    [PipelineDisplayName("DoActionRemoveEnvironment")]
    public class DoActionRemoveEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionRemoveEnvironment"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionRemoveEnvironment(
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
                || !entityView.Action.Equals("DevOps-RemoveEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var pluginPolicy = context.GetPolicy<PluginPolicy>();

                var name = entityView.ItemId.Replace("Entity-CommerceEnvironment-", "");

                var storedEnvironment = await this._commerceCommander.Command<GetEnvironmentCommand>()
                    .Process(this._commerceCommander.GetGlobalContext(context.CommerceContext), name);

                //Access to ensure it exists
                var logicalDeleteComponent = storedEnvironment.GetComponent<LogicalDeleteComponent>();

                //Remove Environment from any lists
                foreach(var membership in storedEnvironment.GetComponent<ListMembershipsComponent>().Memberships)
                {
                    await this._commerceCommander.Command<ListCommander>()
                        .RemoveItemsFromList(this._commerceCommander.GetGlobalContext(context.CommerceContext), membership, new List<string>() { storedEnvironment.Id });
                    //Remove Entity from List

                }

                storedEnvironment.GetComponent<ListMembershipsComponent>().Memberships.Clear();
                storedEnvironment.GetComponent<ListMembershipsComponent>().Memberships.Add("Core.RecycleBin");

                var persistResult = await this._commerceCommander.PersistGlobalEntity(context.CommerceContext,storedEnvironment);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"DevOps.DoActionRemoveEnvironment.Exception: Message={ex.Message}");
            }

            return entityView;
        }
   }
}
