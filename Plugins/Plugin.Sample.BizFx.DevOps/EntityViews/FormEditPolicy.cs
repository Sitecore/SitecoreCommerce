// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormEditPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System.Linq;
    using System;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core.Commands;

    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("FormEditPolicy")]
    public class FormEditPolicy : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormEditPolicy"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FormEditPolicy(
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

            if (entityView.Name != "DevOps-EditPolicy")
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityViewArgument.Entity == null)
            {
                //return Task.FromResult(entityView);

                var name = entityView.EntityId.Replace("Entity-CommerceEnvironment-", "");

                try
                {
                    entityViewArgument.Entity = await this._commerceCommander.Command<GetEnvironmentCommand>()
                        .Process(this._commerceCommander.GetGlobalContext(context.CommerceContext), name);

                    var selectedPolicy = entityViewArgument.Entity.Policies.FirstOrDefault(p => p.PolicyId == entityView.ItemId);
                    if (selectedPolicy != null)
                    {
                        var props = selectedPolicy.GetType().GetProperties();
                        foreach (var prop in props)
                        {
                            if (prop.Name == "Models" || prop.Name == "PolicyId")
                            {
                                //Do nothing
                            }
                            
                            else
                            {

                                var originalType = prop.PropertyType.FullName;
                                if (originalType == "Sitecore.Commerce.Core.EntityReference")
                                {
                                    context.Logger.LogInformation($"DevOps.DoActionEditPolicy.CantProcessProperty: Name={prop.Name}|OriginalType={originalType}");
                                }
                                else if (originalType.Contains("System.Collections.Generic.List"))
                                {
                                    //do nothing, ignore these
                                    context.Logger.LogInformation($"DevOps.DoActionEditPolicy.CantProcessProperty: Name={prop.Name}|OriginalType={originalType}");
                                }
                                else
                                {
                                    entityView.Properties.Add(
                                    new ViewProperty
                                    {
                                        Name = prop.Name,
                                        IsHidden = false,
                                        //IsReadOnly = true,
                                        IsRequired = false,
                                        RawValue = prop.GetValue(selectedPolicy, null)
                                    });
                                }

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    //context.Logger.LogInformation($"Content.Synchronize.DeleteEntity: Id={entityContentItem.Id}");
                    context.Logger.LogError($"Content.SynchronizeContentPath.PathNotFound: Message={ex.Message}");
                }

            }

            return entityView;
        }


    }

}
