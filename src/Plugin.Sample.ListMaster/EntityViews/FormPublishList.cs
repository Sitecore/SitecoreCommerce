namespace Plugin.Sample.ListMaster.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("FormExportList")]
    public class FormPublishList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormPublishList(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "ListMaster_ExportList")
            {
                return entityView;
            }
            
            var managedList = await this._commerceCommander.Command<GetManagedListCommand>().Process(context.CommerceContext, entityView.ItemId.Replace("Entity-ManagedList-", ""));

            var listCount = managedList.TotalItemCount;

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "List",
                    IsHidden = false,
                    IsReadOnly = true,
                    IsRequired = false,
                    RawValue = entityView.ItemId
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Count",
                    IsHidden = false,
                    IsReadOnly = true,
                    IsRequired = false,
                    RawValue = listCount
                });
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "AsPack",
                    IsHidden = false,
                    IsReadOnly = false,
                    IsRequired = false,
                    RawValue = true
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Incremental",
                    IsHidden = false,
                    IsReadOnly = false,
                    IsRequired = false,
                    RawValue = true
                });

            return entityView;
        }
    }
}
