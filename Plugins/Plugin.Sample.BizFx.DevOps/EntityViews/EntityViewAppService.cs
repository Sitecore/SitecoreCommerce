// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityViewAppService.cs" company="Sitecore Corporation">
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
    [PipelineDisplayName("EntityViewAppService")]
    public class EntityViewAppService : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityViewAppService"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EntityViewAppService(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            //Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView == null) return entityView;

            if (entityView.EntityId == null || !entityView.EntityId.Contains("Entity-AppService-"))
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;

            var pathsView = new EntityView
            {
                EntityId = "",
                ItemId = "Id",
                DisplayName = "Messages",
                Name = "Messages",
                UiHint = "Table"
            };

            foreach(var message in entityViewArgument.Entity.GetComponent<ActionHistoryComponent>().History)
            {
                var pathsView1 = new EntityView
                {
                    EntityId = "",
                    ItemId = "Id",
                    DisplayName = "Connection Paths",
                    Name = "Connection Paths",
                    UiHint = ""
                };
                pathsView.ChildViews.Add(pathsView1);

                pathsView1.Properties
                .Add(new ViewProperty { Name = "Name", RawValue = message.Name });
                pathsView1.Properties
                .Add(new ViewProperty { Name = "Date", RawValue = message.Completed });
                pathsView1.Properties
                .Add(new ViewProperty { Name = "Response", RawValue = message.Response });
                pathsView1.Properties
                .Add(new ViewProperty { Name = "EntityId", RawValue = message.EntityId });
                pathsView1.Properties
                .Add(new ViewProperty { Name = "ItemId", RawValue = message.ItemId });

                if (message.Response != "Ok")
                {
                    pathsView1.Properties
                    .Add(new ViewProperty { Name = "JSON", RawValue = message.JSON });
                }
            }

            entityView.ChildViews.Add(pathsView);

            var appServices = await this._commerceCommander.Command<ListCommander>()
                .GetListItems<AppService>(context.CommerceContext, "AppServices", 0, 999);

            return entityView;
        }


        private void AddAsLineItem(EntityView parentView, ViewProperty property)
        {
            var pathsView1 = new EntityView
            {
                EntityId = "",
                ItemId = "Id",
                DisplayName = "Connection Paths",
                Name = "Connection Paths",
                UiHint = "List"
            };
            parentView.ChildViews.Add(pathsView1);
            pathsView1.Properties
                .Add(new ViewProperty { Name = "Path", RawValue = property.Name });
            pathsView1.Properties
                .Add(property);
        }

        private async Task<string> AddServiceHosts(EntityView entityView, CommerceContext commerceContext)
        {
            var masterListEntityView = new EntityView
            {
                EntityId = "",
                ItemId = "",
                DisplayName = "Resources",
                Name = "AppServices",
                UiHint = "Table"
            };
            entityView.ChildViews.Add(masterListEntityView);

            var appServices = await this._commerceCommander.Command<ListCommander>()
                .GetListItems<AppService>(commerceContext, "AppServices", 0, 999);

            foreach (var appService in appServices.OfType<AppService>())
            {
                var childView = new EntityView
                {
                    EntityId = "",
                    ItemId = appService.Id,
                    DisplayName = appService.Name,
                    Name = appService.Name
                };
                masterListEntityView.ChildViews.Add(childView);

                childView.Properties.Add(new ViewProperty { Name = "Name", RawValue = appService.Name, UiType = "EntityLink" });
                childView.Properties.Add(new ViewProperty { Name = "Type", RawValue = appService.ServiceType });
                childView.Properties.Add(new ViewProperty { Name = "Host", RawValue = appService.Host });
                childView.Properties.Add(new ViewProperty { Name = "Description", RawValue = appService.Description });
            }





            //var childView2 = new EntityView
            //{
            //    EntityId = "",
            //    ItemId = "ServiceHost02",
            //    DisplayName = "ServiceHost02",
            //    Name = "ServiceHost02",
            //    //Policies = new List<Policy>() { new ViewImagePolicy { ImageUrl = @".\src\images\SoftwareV2\48x48\application_server.png" } }
            //};

            //childView2.Properties.Add(new ViewProperty { Name = "Name", RawValue = "ServiceHost02" });
            //childView2.Properties.Add(new ViewProperty { Name = "Type", RawValue = "Azure" });
            //childView2.Properties.Add(new ViewProperty { Name = "Description", RawValue = "Azure Web App" });

            //masterListEntityView.ChildViews.Add(childView2);

            return "";
        }

        private void AddTopologies(EntityView entityView, CommerceContext commerceContext)
        {
            var masterListEntityView = new EntityView
            {
                EntityId = "",
                ItemId = "",
                DisplayName = "topologies View",
                Name = "TopologiesView",
                UiHint = "List"
            };
            entityView.ChildViews.Add(masterListEntityView);

            var childView = new EntityView
            {
                EntityId = "",
                ItemId = "Topology01",
                DisplayName = "Topology01",
                Name = "Topology01",
                //Policies = new List<Policy>() { new ViewImagePolicy { ImageUrl = @".\src\images\NetworkV2\48x48\server_to_client.png" } }
            };

            childView.Properties.Add(new ViewProperty { Name = "Name", RawValue = "Topology01" });
            childView.Properties.Add(new ViewProperty { Name = "Type", RawValue = "OneBox" });
            childView.Properties.Add(new ViewProperty { Name = "Description", RawValue = "OneBox Topology" });

            masterListEntityView.ChildViews.Add(childView);

            var childView2 = new EntityView
            {
                EntityId = "",
                ItemId = "Topology02",
                DisplayName = "Topology02",
                Name = "Topology02",
                //Policies = new List<Policy>() { new ViewImagePolicy { ImageUrl = @".\src\images\NetworkV2\48x48\server_to_client.png" } }
            };

            childView2.Properties.Add(new ViewProperty { Name = "Name", RawValue = "Topology02" });
            childView2.Properties.Add(new ViewProperty { Name = "Type", RawValue = "SimpleHa" });
            childView2.Properties.Add(new ViewProperty { Name = "Description", RawValue = "Simple HA configuration" });

            masterListEntityView.ChildViews.Add(childView2);
        }

    }
}
