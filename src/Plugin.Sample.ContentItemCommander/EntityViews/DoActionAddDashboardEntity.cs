namespace Plugin.Sample.ContentItemCommander.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.ContentItemCommander.Entities;

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
                || !entityView.Action.Equals("MyDashboard-AddDashboardEntity", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var name = entityView.Properties.First(p => p.Name == "Name").Value ?? "";
                var displayName = entityView.Properties.First(p => p.Name == "DisplayName").Value ?? "";

                var sampleDashboardEntity = new SampleDashboardEntity {Id = CommerceEntity.IdPrefix<SampleDashboardEntity>() + Guid.NewGuid().ToString("N"), Name = name, DisplayName = displayName };

                sampleDashboardEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<SampleDashboardEntity>());

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
