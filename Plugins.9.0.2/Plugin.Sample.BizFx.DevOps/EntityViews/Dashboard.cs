// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dashboard.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Microsoft.Extensions.Logging;


    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Dashboard")]
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dashboard"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public Dashboard(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "DevOps-Dashboard")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "Dev Ops";

            try
            {
                this._commerceCommander.Command<ChildViewEnvironmentDetails>().Process(context.CommerceContext, entityView, context.CommerceContext.Environment);

                await this._commerceCommander.Command<ChildViewResources>().Process(context.CommerceContext, entityView);

                await this._commerceCommander.Command<ChildViewEnvironments>().Process(context.CommerceContext, entityView);

                this._commerceCommander.Command<ChildViewDevOpsConnectionPaths>().Process(context.CommerceContext, entityView);
                this._commerceCommander.Command<ChildViewDevOpsTopologies>().Process(context.CommerceContext, entityView);
            }
            catch(Exception ex)
            {
                context.Logger.LogError(ex, "DevOps.DashBoard.Exception");
            }
            return entityView;
        }

    }
}
