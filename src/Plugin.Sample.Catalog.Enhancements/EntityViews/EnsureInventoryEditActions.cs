namespace Plugin.Sample.Catalog.Enhancements.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("EnsureInventoryEditActions")]
    public class EnsureInventoryEditActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView != null)
            {
                if (entityView.EntityId.Contains("Entity-SellableItem-") && string.IsNullOrEmpty(entityView.Action))
                {
                    var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();
                    
                    actionsPolicy.Actions.Add(new EntityActionView
                    {
                        Name = "EditSellableItemInventory",
                        DisplayName = "Edit Inventory",
                        Description = "Adds a new Sample Dashboard Entity",
                        IsEnabled = true,
                        RequiresConfirmation = false, ConfirmationMessage = "This is a confirmation message",
                        EntityView = "EditInventory",
                        Icon = "add"
                    });
                }
            }

            return Task.FromResult(entityView);
        }
    }
}
