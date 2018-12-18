namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionRemoveEnvironment")]
    public class DoActionRemoveEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionRemoveEnvironment(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-RemoveEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var name = entityView.ItemId.Replace("Entity-CommerceEnvironment-", "");

                var storedEnvironment = await this._commerceCommander.Command<GetEnvironmentCommand>()
                    .Process(this._commerceCommander.GetGlobalContext(context.CommerceContext), name);
               
                foreach(var membership in storedEnvironment.GetComponent<ListMembershipsComponent>().Memberships)
                {
                    await this._commerceCommander.Command<ListCommander>()
                        .RemoveItemsFromList(this._commerceCommander.GetGlobalContext(context.CommerceContext), membership, new List<string>() { storedEnvironment.Id });
                }

                storedEnvironment.GetComponent<ListMembershipsComponent>().Memberships.Clear();
                storedEnvironment.GetComponent<ListMembershipsComponent>().Memberships.Add("Core.RecycleBin");

                await this._commerceCommander.PersistGlobalEntity(context.CommerceContext,storedEnvironment);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"DevOps.DoActionRemoveEnvironment.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
