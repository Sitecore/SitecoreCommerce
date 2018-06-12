
using System.Threading.Tasks;

namespace Plugin.Sample.ListMaster
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("FormExportList")]
    public class FormPublishList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormPublishList"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FormPublishList(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>Runs the Command.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "ListMaster_ExportList")
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            //var listCount = await this._commerceCommander.Command<GetListCountCommand>().Process(context.CommerceContext, entityView.ItemId.Replace("Entity-ManagedList-",""));
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

            //entityView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "AsFiles",
            //        IsHidden = false,
            //        IsReadOnly = false,
            //        IsRequired = false,
            //        RawValue = false
            //    });

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
