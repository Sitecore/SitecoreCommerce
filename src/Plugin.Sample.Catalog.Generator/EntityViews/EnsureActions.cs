namespace Plugin.Sample.Catalog.Generator.EntityViews
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

            if (entityView.Name == "MerchandisingDashboard")
            {
                var catalogsEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Catalogs");

                catalogsEntityView?.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                {
                    Name = "Catalog-GenerateSampleCatalog",
                    DisplayName = $"Generate Sample Catalog",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "Catalog-GenerateSampleCatalog",
                    UiHint = string.Empty
                });

                return Task.FromResult(entityView);
            }

            return Task.FromResult(entityView);
        }
    }
}
