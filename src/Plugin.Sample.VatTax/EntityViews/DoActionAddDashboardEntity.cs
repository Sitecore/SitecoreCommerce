namespace Plugin.Sample.VatTax.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.VatTax.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionAddDashboardEntity")]
    public class DoActionAddDashboardEntity : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionAddDashboardEntity(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("VatTaxDashboard-AddDashboardEntity", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var taxTag = entityView.Properties.First(p => p.Name == "TaxTag").Value ?? "";
                var countryCode = entityView.Properties.First(p => p.Name == "CountryCode").Value ?? "";
                var taxPct = System.Convert.ToDecimal(entityView.Properties.First(p => p.Name == "TaxPct").Value ?? "0");

                var sampleDashboardEntity = new VatTaxTableEntity
                {
                    Id = CommerceEntity.IdPrefix<VatTaxTableEntity>() + Guid.NewGuid().ToString("N"),
                    Name = string.Empty, TaxTag = taxTag,
                    CountryCode = countryCode,
                    TaxPct = taxPct
                };

                sampleDashboardEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<VatTaxTableEntity>());

                await this._commerceCommander.PersistEntity(context.CommerceContext, sampleDashboardEntity);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
