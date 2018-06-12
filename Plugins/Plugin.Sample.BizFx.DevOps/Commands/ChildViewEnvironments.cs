// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetContentItemCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the SampleCommand command.
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
    /// Defines the ChildViewEnvironments command.
    /// </summary>
    public class ChildViewEnvironments : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildViewEnvironments"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ChildViewEnvironments(IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="entityView">
        /// The entityView for the command
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<EntityView> Process(CommerceContext commerceContext, EntityView entityView)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var pluginPolicy = commerceContext.GetPolicy<PluginPolicy>();

                try
                {
                    var environmentListEntityView = new EntityView
                    {
                        EntityId = "",
                        ItemId = "",
                        DisplayName = "Environments List View",
                        Name = "DevOps-ListView-Environments",
                        UiHint = "Table",
                        Icon = pluginPolicy.Icon
                    };
                    entityView.ChildViews.Add(environmentListEntityView);

                    var environments = await this._commerceCommander.Command<ListCommander>()
                        .GetListItems<CommerceEnvironment>(this._commerceCommander.GetGlobalContext(commerceContext), 
                        CommerceEntity.ListName<CommerceEnvironment>(), 0, 99);

                    foreach (var environment in environments)
                    {

                        var environmentView = new EntityView
                        {
                            EntityId = "",
                            ItemId = environment.Id,
                            DisplayName = environment.Name,
                            Name = environment.Name,
                            Icon = pluginPolicy.Icon
                        };

                        environmentView.Properties.Add(new ViewProperty { Name = "Name", RawValue = environment.Name, UiType = "EntityLink" });
                        environmentView.Properties.Add(new ViewProperty { Name = "ArtifactId", RawValue = environment.ArtifactStoreId.ToString("N") });
                        //environmentView.Properties.Add(new ViewProperty { Name = "DisplayName", RawValue = environment.DisplayName });
                        environmentView.Properties.Add(new ViewProperty { Name = "DateCreated", RawValue = environment.DateCreated });

                        environmentView.Properties
                            .Add(new ViewProperty { Name = "Policies", RawValue = environment.Policies.Count });
                        environmentView.Properties
                            .Add(new ViewProperty { Name = "Components", RawValue = environment.Components.Count });

                        environmentListEntityView.ChildViews.Add(environmentView);
                    }
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