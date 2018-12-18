namespace Plugin.Sample.Messaging.EntityViews
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
            
            if (entityView.Name != "MessagesDashboard")
            {
                return Task.FromResult(entityView);
            }
            var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "MessagingView");

            if (tableEntityView != null)
            {
                var tableEntityViewActionsPolicy = tableEntityView.GetPolicy<ActionsPolicy>();
                
                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "MessagesDashboard-ClearMessages",
                    DisplayName = "Clear All Messages",
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
