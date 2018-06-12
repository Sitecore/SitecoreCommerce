
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Plugin.Sample.Messaging
{
    
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System.Linq;


    /// <summary>
    /// Defines a block which populates an EntityView for a Sample Page in the Sitecore Commerce Focused Commerce Experience.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Dashboard")]
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dashboard"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public Dashboard(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "MessagesDashboard")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "Messages";
            entityView.DisplayName = "My Messages";

            //entityView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "ViewProperty1",
            //        IsHidden = false,
            //        IsReadOnly = true,
            //        RawValue = "ValueString"
            //    });

            //entityView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "HtmlProperty",
            //        IsHidden = false,
            //        IsReadOnly = true,
            //        OriginalType = "Html",
            //        UiType = "Html",
            //        RawValue = "<b>ValueString</b>"
            //    });


            var messages = await this._commerceCommander.Command<ListCommander>().GetListItems<MessageEntity>(context.CommerceContext, "MessageEntities", 0, 20);

            var newEntityViewTable = new EntityView
            {
                Name = "MessagingView",
                UiHint = "Table",
                Icon = pluginPolicy.Icon,
                ItemId = "",
            };
            entityView.ChildViews.Add(newEntityViewTable);

            //var sampleDashboardEntities = await this._commerceCommander.Command<ListCommander>()
            //            .GetListItems<MessageEntity>(context.CommerceContext,
            //            CommerceEntity.ListName<MessageEntity>(), 0, 99);
            foreach (var message in messages)
            {
                newEntityViewTable.ChildViews.Add(
                    new EntityView
                    {
                        ItemId = message.Id,
                        Icon = pluginPolicy.Icon,
                        Properties = new List<ViewProperty> {
                            new ViewProperty {Name = "ItemId", RawValue = message.Id, UiType = "EntityLink", IsHidden = true },
                            new ViewProperty {Name = "Name", RawValue = message.Name, UiType = "EntityLink" },
                            new ViewProperty {Name = "Message", RawValue = message.History.First().EventMessage, OriginalType = "Html", UiType = "Html" }
                        }
                    }
                );
            }

            return entityView;
        }
    }
}
