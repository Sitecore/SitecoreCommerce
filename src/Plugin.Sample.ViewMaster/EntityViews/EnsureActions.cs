namespace Plugin.Sample.ViewMaster.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Plugin.Enhancements.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.ViewMaster.Policies.PluginPolicy;

    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public EnsureActions(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.Name != "ViewMaster")
            {
                return entityView;
            }

            await this._commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, this._commerceCommander);
            
            var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Views Recorded");
            if (tableEntityView != null)
            {
                var tableEntityViewActionsPolicy = tableEntityView.GetPolicy<ActionsPolicy>();

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ViewMaster-ClearEvents",
                    DisplayName = "Clear All Events",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    Icon = "delete"
                });
                
                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ViewMaster-StartRecording",
                    DisplayName = "Start Recording",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    Icon = "delete"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ViewMaster-StopRecording",
                    DisplayName = "Stop Recording",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    Icon = "delete"
                });
            }

            return entityView;
        }
    }
}
