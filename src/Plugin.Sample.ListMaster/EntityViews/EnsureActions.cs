namespace Plugin.Sample.ListMaster.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "ListMaster")
            {
                return Task.FromResult(entityView);
            }

            if (entityView != null)
            {
                var tableEntityViewActionsPolicy = entityView.GetPolicy<ActionsPolicy>();

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-AddList",
                    DisplayName = "Add A List",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "ListMaster-FormAddList",
                    Icon = "add"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-DeleteList",
                    DisplayName = "Delete A List",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    Icon = "delete"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-ExportList",
                    DisplayName = "Publish List",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "ListMaster_ExportList",
                    Icon = "escalator_up"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-ImportList",
                    DisplayName = "Import List",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "ListMaster_ImportList",
                    Icon = "escalator_down"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-SetPublishPath",
                    DisplayName = "Set Publish Path",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "ListMaster_SetPublishPath",
                    Icon = "escalator_down"
                });
            }

            return Task.FromResult(entityView);
        }
    }
}
