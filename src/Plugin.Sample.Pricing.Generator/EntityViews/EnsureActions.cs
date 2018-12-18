namespace Plugin.Sample.Pricing.Generator.EntityViews
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

            if (entityView.Name == "PricingDashboard")
            {
                var priceBooksEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "PriceBooks");

                if (priceBooksEntityView != null)
                {
                    priceBooksEntityView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                    {
                        Name = "Pricing-GenerateSamplePriceBook",
                        DisplayName = $"Generate Sample PriceBook",
                        Description = string.Empty,
                        IsEnabled = true,
                        RequiresConfirmation = false,
                        EntityView = "Pricing-GenerateSamplePriceBook",
                        Icon = "Add"
                    });

                    priceBooksEntityView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                    {
                        Name = "Pricing-DeletePriceBook",
                        DisplayName = $"Delete PriceBook",
                        Description = string.Empty,
                        IsEnabled = true,
                        RequiresConfirmation = true,
                        EntityView = string.Empty,
                        Icon = "delete"
                    });

                    priceBooksEntityView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                    {
                        Name = "Pricing-ClearPriceCards",
                        DisplayName = $"Clear Price Cards",
                        Description = string.Empty,
                        IsEnabled = true,
                        RequiresConfirmation = true,
                        EntityView = string.Empty,
                        Icon = "delete"
                    });
                }

                return Task.FromResult(entityView);
            }

            return Task.FromResult(entityView);
        }
    }
}
