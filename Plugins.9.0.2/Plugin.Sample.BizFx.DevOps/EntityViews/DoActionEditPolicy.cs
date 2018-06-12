// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionEditPolicy.cs" company="Sitecore Corporation">
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
    [PipelineDisplayName("DoActionEditPolicy")]
    public class DoActionEditPolicy : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionEditPolicy"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionEditPolicy(
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
                || !entityView.Action.Equals("DevOps-EditPolicy", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var pluginPolicy = context.GetPolicy<PluginPolicy>();

                var name = entityView.EntityId.Replace("Entity-CommerceEnvironment-", "");

                try
                {
                    var storedEntity = await this._commerceCommander.Command<GetEnvironmentCommand>()
                        .Process(this._commerceCommander.GetGlobalContext(context.CommerceContext), name);

                    var selectedPolicy = storedEntity.Policies.FirstOrDefault(p => p.PolicyId == entityView.ItemId);
                    if (selectedPolicy != null)
                    {
                        foreach (var prop in entityView.Properties)
                        {
                            if (prop.Name == "Models" || prop.Name == "PolicyId")
                            {
                                //Do nothing
                            }
                            else
                            {
                                var policyProperties = selectedPolicy.GetType().GetProperties();

                                var policyProperty = policyProperties.First(p => p.Name == prop.Name);

                                if (prop.OriginalType == "System.Decimal")
                                {
                                    policyProperty.SetValue(selectedPolicy, System.Convert.ToDecimal(prop.Value));
                                }
                                else if (prop.OriginalType == "System.Boolean")
                                {
                                    policyProperty.SetValue(selectedPolicy, System.Convert.ToBoolean(prop.Value));
                                }
                                else if (prop.OriginalType == "Sitecore.Commerce.Core.EntityReference")
                                {
                                    context.Logger.LogInformation($"DevOps.DoActionEditPolicy.CantProcessProperty: Name={prop.Name}|OriginalType={prop.OriginalType}");
                                }
                                else if (prop.OriginalType.Contains("System.Collections.Generic.List"))
                                {
                                    //do nothing, ignore these
                                    context.Logger.LogInformation($"DevOps.DoActionEditPolicy.CantProcessProperty: Name={prop.Name}|OriginalType={prop.OriginalType}");
                                }
                                else
                                {
                                    policyProperty.SetValue(selectedPolicy, prop.Value);
                                }
                            }

                        }
                    }

                    var persistResult = await this._commerceCommander.PersistGlobalEntity(context.CommerceContext, storedEntity);

                }
                catch (Exception ex)
                {
                    //context.Logger.LogInformation($"Content.Synchronize.DeleteEntity: Id={entityContentItem.Id}");
                    context.Logger.LogError($"{this.Name}.PathNotFound: Message={ex.Message}");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"{this.Name}.Exception: Message={ex.Message}");
            }

            return entityView;
        }
   }
}
