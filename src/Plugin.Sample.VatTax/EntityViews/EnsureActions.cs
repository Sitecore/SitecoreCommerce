namespace Plugin.Sample.VatTax.EntityViews
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
            
            if (entityView.Name != "VatTaxDashboard")
            {
                return Task.FromResult(entityView);
            }

            var tableViewActionsPolicy = entityView.GetPolicy<ActionsPolicy>();
            tableViewActionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "VatTaxDashboard-AddDashboardEntity",
                DisplayName = "Adds a new Vat Tax Entry",
                Description = "Adds a new Vat Tax Entry",
                IsEnabled = true,
                RequiresConfirmation = false,
                EntityView = "VatTaxDashboard-FormAddDashboardEntity",
                Icon = "add"
            });

            tableViewActionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "VatTaxDashboard-RemoveDashboardEntity",
                DisplayName = "Remove Vat Tax Entry",
                Description = "Removes a Vat Tax Entry",
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = string.Empty,
                Icon = "delete"
            });

            return Task.FromResult(entityView);
        }
    }
}
