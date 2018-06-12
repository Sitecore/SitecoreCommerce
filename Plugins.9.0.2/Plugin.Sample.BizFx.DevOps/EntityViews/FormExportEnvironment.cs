// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormExportEnvironment.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System.Threading.Tasks;
    using System.Linq;

    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("FormExportEnvironment")]
    public class FormExportEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormPullEnvironment"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FormExportEnvironment(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "DevOps-ExportEnvironment")
            {
                return Task.FromResult(entityView);
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityViewArgument.Entity == null)
            {

                //var appService = (await this._findEntityPipeline.Run(new FindEntityArgument(typeof(CommerceEntity), entityView.ItemId, false), context.CommerceContext.GetPipelineContextOptions())) as AppService;

                //if (appService == null)
                //{
                //    //not found
                //}
                //else
                //{

                    //var serviceUri = $"http://{appService.Host}/commerceops/Environments";

                    //try
                    //{
                    //    var jsonResponse = await this._jsonCommander.Process(context.CommerceContext, serviceUri);

                    //    dynamic dynJson = JsonConvert.DeserializeObject(jsonResponse.Json);

                    //    var environments = dynJson.value;


                    //    //foreach (var environment in environments)
                    //    //{
                    //    //    Console.WriteLine("{0} {1}\n", environment.Name, environment.DisplayName);
                    //    //}

                    //    var templateViewProperty = new ViewProperty
                    //    {
                    //        Name = "Environment",
                    //        DisplayName = "Selected Environment",
                    //        IsHidden = false,
                    //        //IsReadOnly = true,
                    //        IsRequired = true,
                    //        RawValue = "",

                    //    };

                    //    entityView.Properties.Add(templateViewProperty);

                    //    var availableSelections = templateViewProperty.GetPolicy<AvailableSelectionsPolicy>();

                    //    foreach (var environment in environments)
                    //    {
                    //        availableSelections.List.Add(new Selection { Name = environment.Name, DisplayName = environment.Name, IsDefault = false });
                    //    }



                    //    entityView.Properties.Add(new ViewProperty
                    //    {
                    //        Name = "NameAs",
                    //        DisplayName = "Name Environment As",
                    //        IsHidden = false,
                    //        //IsReadOnly = true,
                    //        IsRequired = false,
                    //        RawValue = "",

                    //    });

                    //}
                    //catch (Exception ex)
                    //{
                    //    //context.Logger.LogInformation($"Content.Synchronize.DeleteEntity: Id={entityContentItem.Id}");
                    //    context.Logger.LogError($"DevOps.FormPullEnvironment.Exception: Message={ex.Message}");
                    //}




                //}



            }
            return Task.FromResult(entityView);
        }


    }

}
