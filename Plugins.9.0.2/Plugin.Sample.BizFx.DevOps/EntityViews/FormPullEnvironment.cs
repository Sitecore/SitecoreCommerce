// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormPullEnvironment.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Plugin.Sample.JsonCommander;

    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("FormPullEnvironment")]
    public class FormPullEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormPullEnvironment"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FormPullEnvironment(CommerceCommander commerceCommander)
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

            if (entityView.Name != "DevOps-PullEnvironment")
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<Sitecore.Commerce.EntityViews.ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityViewArgument.Entity == null)
            {

                var appService = await this._commerceCommander
                    .GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

                if (appService == null)
                {
                    //not found
                }
                else
                {

                    var serviceUri = $"http://{appService.Host}/commerceops/Environments";

                    try
                    {
                        var jsonResponse = await this._commerceCommander.Command<JsonCommander>()
                            .Process(context.CommerceContext, serviceUri);

                        dynamic dynJson = JsonConvert.DeserializeObject(jsonResponse.Json);

                        var environments = dynJson.value;

                        var templateViewProperty = new ViewProperty
                        {
                            Name = "Environment",
                            DisplayName = "Selected Environment",
                            IsHidden = false,
                            //IsReadOnly = true,
                            IsRequired = true,
                            RawValue = "",

                        };

                        entityView.Properties.Add(templateViewProperty);

                        var availableSelections = templateViewProperty.GetPolicy<AvailableSelectionsPolicy>();

                        foreach (var environment in environments)
                        {
                            availableSelections.List.Add(new Selection { Name = environment.Name, DisplayName = environment.Name, IsDefault = false });
                        }



                        entityView.Properties.Add(new ViewProperty
                        {
                            Name = "NameAs",
                            DisplayName = "Name Environment As",
                            IsHidden = false,
                            //IsReadOnly = true,
                            IsRequired = false,
                            RawValue = "",

                        });

                    }
                    catch (Exception ex)
                    {
                        //context.Logger.LogInformation($"Content.Synchronize.DeleteEntity: Id={entityContentItem.Id}");
                        context.Logger.LogError($"DevOps.FormPullEnvironment.Exception: Message={ex.Message}");
                    }
                }
            }
            return entityView;
        }


    }

}
