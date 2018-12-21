namespace Plugin.Sample.Catalog.Generator.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("FormGenerateSampleCatalog")]
    public class FormGenerateSampleCatalog : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Catalog-GenerateSampleCatalog")
            {
                return Task.FromResult(entityView);
            }
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Name",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DisplayName",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Categories",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = 100
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Products",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = 100
                });

            return Task.FromResult(entityView);
        }
    }

}
