namespace Plugin.Sample.ListMaster.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("DoActionDeleteList")]
    public class DoActionDeleteList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionDeleteList(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("ListMaster-DeleteList", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                await this._commerceCommander.Command<ListCommander>()
                    .RemoveItemsFromList(context.CommerceContext, "ManagedLists", new List<string>() { entityView.ItemId });

                await this._commerceCommander.DeleteEntity(context.CommerceContext, entityView.ItemId);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddList.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
