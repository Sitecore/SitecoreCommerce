// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionExportEnvironment.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

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
    [PipelineDisplayName("DoActionExportEnvironment")]
    public class DoActionExportEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        private static readonly JsonSerializerSettings Serializer = new JsonSerializerSettings
        {  
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionExportEnvironment"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionExportEnvironment(
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
                || !entityView.Action.Equals("DevOps-ExportEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var pluginPolicy = context.GetPolicy<PluginPolicy>();

                var name = entityView.ItemId.Replace("Entity-CommerceEnvironment-", "");

                var storedEnvironment = await this._commerceCommander.Command<GetEnvironmentCommand>()
                    .Process(this._commerceCommander.GetGlobalContext(context.CommerceContext), name);


                var exportEnvironmentResult = await this._commerceCommander.Command<ExportEnvironmentCommand>()
                    .Process(context.CommerceContext, name);

                var path = this._commerceCommander.CurrentNodeContext(context.CommerceContext).WebRootPath + @"\data\environments";

                File.WriteAllText(path + $@"\" + name + ".json", exportEnvironmentResult);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"DevOps.DoActionPullEnvironment.Exception: Message={ex.Message}");
            }

            return entityView;
        }
   }
}
