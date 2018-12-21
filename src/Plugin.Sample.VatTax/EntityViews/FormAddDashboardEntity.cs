
namespace Plugin.Sample.VatTax.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("FormAddDashboardEntity")]
    public class FormAddDashboardEntity : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "VatTaxDashboard-FormAddDashboardEntity")
            {
                return Task.FromResult(entityView);
            }

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "TaxTag",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = string.Empty
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "CountryCode",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = string.Empty
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "TaxPct",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = 0
                });

            return Task.FromResult(entityView);
        }


    }

}
