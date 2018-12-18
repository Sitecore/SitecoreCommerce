namespace Plugin.Sample.ListMaster.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionAddDashboardEntity")]
    public class DoActionAddList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionAddList(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("ListMaster-AddList", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var list = entityView.Properties.First(p => p.Name == "List").Value ?? "";
                var asManagedList = System.Convert.ToBoolean(entityView.Properties.First(p => p.Name == "AsManagedList").Value);

                var listId = string.Empty;
                listId = asManagedList ? $"Entity-ManagedList-{list}" : list;

                var managedList = new ManagedList() { Id = listId, Name = list, DisplayName = list };
                managedList.GetComponent<ListMembershipsComponent>().Memberships.Add("ManagedLists");
                await this._commerceCommander.PersistEntity(context.CommerceContext, managedList);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddList.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
