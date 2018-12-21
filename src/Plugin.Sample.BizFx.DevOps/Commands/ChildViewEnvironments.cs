namespace Plugin.Sample.BizFx.DevOps.Commands
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;

    using PluginPolicy = Plugin.Sample.BizFx.DevOps.Policies.PluginPolicy;

    public class ChildViewEnvironments : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public ChildViewEnvironments(IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }

        public async Task<EntityView> Process(CommerceContext commerceContext, EntityView entityView)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var pluginPolicy = commerceContext.GetPolicy<PluginPolicy>();

                try
                {
                    var environmentListEntityView = new EntityView
                    {
                        EntityId = string.Empty,
                        ItemId = string.Empty,
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
                            EntityId = string.Empty,
                            ItemId = environment.Id,
                            DisplayName = environment.Name,
                            Name = environment.Name,
                            Icon = pluginPolicy.Icon
                        };

                        environmentView.Properties.Add(new ViewProperty { Name = "Name", RawValue = environment.Name, UiType = "EntityLink" });
                        environmentView.Properties.Add(new ViewProperty { Name = "ArtifactId", RawValue = environment.ArtifactStoreId.ToString("N") });
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