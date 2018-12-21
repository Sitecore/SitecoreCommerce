namespace Plugin.Sample.CommerceDashboard.EntityViews
{
    using System.Linq;
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

            if (entityView.Name != "MyDashboard")
            {
                return Task.FromResult(entityView);
            }

            var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Example Table View");

            if (tableEntityView != null)
            {
                var tableEntityViewActionsPolicy = tableEntityView.GetPolicy<ActionsPolicy>();
                
                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "MyDashboard-AddDashboardEntity",
                    DisplayName = "Adds a new Sample Dashboard Entity",
                    Description = "Adds a new Sample Dashboard Entity",
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "MyDashboard-FormAddDashboardEntity",
                    Icon = "add"
                });
                
                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "MyDashboard-RemoveDashboardEntity",
                    DisplayName = "Removes a Sample Dashboard Entity",
                    Description = "Removes a Sample Dashboard Entity",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    Icon = "delete"
                });
            }

            return Task.FromResult(entityView);
        }
    }
}
