// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildViewEnvironmentDetails.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the ChildViewEnvironmentDetails command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Management;
    using Microsoft.Extensions.Logging;
    using Sitecore.Services.Core.Model;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// Defines the ChildViewEnvironmentDetails command.
    /// </summary>
    public class ChildViewEnvironmentDetails : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildViewEnvironmentDetails"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ChildViewEnvironmentDetails(IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// Processes the EntityView
        /// </summary>
        /// <param name="commerceContext">A CommerceContext</param>
        /// <param name="entityView">The EntityView to Process</param>
        /// <param name="environment">A CommerceEnvironment</param>
        /// <returns>An EntityView</returns>
        public EntityView Process(CommerceContext commerceContext, EntityView entityView, CommerceEnvironment environment)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var pluginPolicy = commerceContext.GetPolicy<PluginPolicy>();

                try
                {
                    if (environment == null) return entityView;

                    //var entityViewEnvironment = new EntityView
                    //{
                    //    EntityId = "",
                    //    ItemId = environment.Id,
                    //    DisplayName = "Environment Display Name",
                    //    Name = "Environment - " + environment.Name,
                    //    Icon = pluginPolicy.Icon
                    //};

                    entityView.Properties
                        .Add(new ViewProperty { Name = "Environment", RawValue = environment.Name, UiType = "EntityLink" });
                    entityView.Properties
                        .Add(new ViewProperty { Name = "Policies", RawValue = environment.Policies.Count });
                    entityView.Properties
                    .Add(new ViewProperty { Name = "Components", RawValue = environment.Components.Count });
                    entityView.Properties
                        .Add(new ViewProperty { Name = "ArtifactStoreId", RawValue = environment.ArtifactStoreId.ToString("N") });
                    entityView.Properties
                        .Add(new ViewProperty { Name = "GlobalEnvironmentName", RawValue = this._commerceCommander.CurrentNodeContext(commerceContext).GlobalEnvironmentName });


                    entityView.GetPolicy<ActionsPolicy>().Actions.Add(
                       new EntityActionView
                       {
                           Name = "DoAction",
                           IsEnabled = true,
                           Description = "Description",
                           DisplayName = "Do Action",
                           RequiresConfirmation = false,
                           UiHint = "RelatedList",
                           EntityView = "/entityView/GetListView-CompletedOrders"
                       });


                    //entityView.ChildViews.Add(entityView);

                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"ChildViewEnvironments.Exception: Message={ex.Message}");
                }
                return entityView;
            }
        }
    }
}