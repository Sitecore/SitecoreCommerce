// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureActions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.CatalogGenerator
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureActions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureActions(CommerceCommander commerceCommander)
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

            if (entityView.Name == "MerchandisingDashboard")
            {
                //var businessUser = await this._commerceCommander.Command<BusinessUserCommander>().CurrentBusinessUser(context.CommerceContext);

                //if (!businessUser.GetPolicy<UserPluginsPolicy>().PlugIns.Any(p => p.PolicyId == this.GetType().Namespace))
                //{
                var catalogsEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Catalogs");

                if (catalogsEntityView != null)
                {
                    catalogsEntityView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                    {
                        Name = "Catalog-GenerateSampleCatalog",
                        DisplayName = $"Generate Sample Catalog",
                        Description = "",
                        IsEnabled = true,
                        RequiresConfirmation = true,
                        EntityView = "Catalog-GenerateSampleCatalog",
                        UiHint = ""
                    });
                }
                //    entityView.ItemId = this.GetType().Namespace;
                return Task.FromResult(entityView);
                //}
            }

            //if (entityView.Name != "MyDashboard")
            //{
            //    return entityView;
            //}

            //var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            ////This action creates an Action that allows you to show a another EntityView
            //actionsPolicy.Actions.Add(new EntityActionView
            //{
            //    Name = "ExampleViewLevelAction",
            //    DisplayName = "Example View Level Action",
            //    Description = "",
            //    IsEnabled = true,
            //    RequiresConfirmation = true,
            //    EntityView = "MyDashboard",
            //    UiHint = ""
            //});


            //var flatEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Example Flat View");

            //if (flatEntityView != null)
            //{
            //    var flatEntityViewActionsPolicy = flatEntityView.GetPolicy<ActionsPolicy>();

            //    //This action creates an Action that allows you to show a another EntityView
            //    flatEntityViewActionsPolicy.Actions.Add(new EntityActionView
            //    {
            //        Name = "ExampleFlatViewAction",
            //        DisplayName = "Example Flat View Action",
            //        Description = "",
            //        IsEnabled = true,
            //        RequiresConfirmation = true,
            //        EntityView = "MyDashboard-MyFlatView",
            //        UiHint = "RelatedList"
            //    });
            //}

            //var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Example Table View");

            //if (tableEntityView != null)
            //{
            //    var tableEntityViewActionsPolicy = tableEntityView.GetPolicy<ActionsPolicy>();

            //    //This action creates an Action that allows you to show a another EntityView
            //    tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
            //    {
            //        Name = "ExampleTableViewAction",
            //        DisplayName = "Example Table View Action",
            //        Description = "",
            //        IsEnabled = true,
            //        RequiresConfirmation = true,
            //        EntityView = "MyDashboard-MyTableView",
            //        UiHint = ""
            //    });
            //}

            return Task.FromResult(entityView);
        }
    }
}
