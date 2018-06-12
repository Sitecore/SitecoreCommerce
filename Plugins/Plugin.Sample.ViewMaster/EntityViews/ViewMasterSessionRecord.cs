
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Plugin.Sample.ViewMaster
{
    using Newtonsoft.Json;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;


    /// <summary>
    /// Defines a block which populates an EntityView for a Sample Page in the Sitecore Commerce Focused Commerce Experience.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("ViewMasterSessionRecord")]
    public class ViewMasterSessionRecord : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMasterSessionRecord"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ViewMasterSessionRecord(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.EntityId != "ViewMaster_Session")
            {
                return Task.FromResult(entityView);
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var session = entityViewArgument.Entity as ViewmasterSessionEntity;

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.Name = "ViewMasterEvent";
            entityView.DisplayName = "ViewMasterEvent";

            var selectedView = session.Views.FirstOrDefault(p => p.Id == entityView.ItemId);

            if (selectedView != null)
            {
                entityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "Name",
                        IsHidden = false,
                        IsReadOnly = true,
                        RawValue = selectedView.View.Name
                    });

                entityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "DisplayName",
                        IsHidden = false,
                        IsReadOnly = true,
                        OriginalType = "Html",
                        UiType = "Html",
                        RawValue = selectedView.View.DisplayName
                    });

                entityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "Entity",
                        IsHidden = false,
                        IsReadOnly = true,
                        OriginalType = "Html",
                        UiType = "Html",
                        RawValue = entityView.EntityId
                    });

                entityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "Item",
                        IsHidden = false,
                        IsReadOnly = true,
                        OriginalType = "Html",
                        UiType = "Html",
                        RawValue = entityView.ItemId
                    });

                var sb = new StringBuilder();
                var sw = new StringWriter(sb);

                JsonSerializer Serializer = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.All,
                    NullValueHandling = NullValueHandling.Ignore
                };

                using (var writer = new JsonTextWriter(sw))
                {
                    Serializer.Serialize(writer, selectedView.View);
                }

                var entityViewAsJson = sw.ToString();

                var entityViewSeparator = new EntityView
                {
                    Name = "Below is View Recorded",
                    UiHint = "Flat",
                    Icon = pluginPolicy.Icon,
                    ItemId = "",
                };
                entityView.ChildViews.Add(entityViewSeparator);

                entityViewSeparator.Properties.Add(
                    new ViewProperty
                    {
                        Name = "Name",
                        IsHidden = false,
                        IsReadOnly = true,
                        RawValue = "================================================================================"
                    });

                entityView.ChildViews.Add(selectedView.View);


                var entityViewRawJson = new EntityView
                {
                    Name = "Raw View",
                    UiHint = "Flat",
                    Icon = pluginPolicy.Icon,
                    ItemId = "",
                };
                entityView.ChildViews.Add(entityViewRawJson);

                entityViewAsJson = Regex.Replace(entityViewAsJson, @"}", "}<br>");

                //entityViewAsJson = entityViewAsJson..Replace("}", "}<br>");

                entityViewRawJson.Properties.Add(
                    new ViewProperty
                    {
                        Name = "ViewAsJson",
                        IsHidden = false,
                        IsReadOnly = true,
                        OriginalType = "Html",
                        UiType = "Html",
                        RawValue = "<b>boo!</b><br>" + entityViewAsJson + ""
                    });

            }

            //var newEntityViewTable = new EntityView
            //{
            //    Name = "Session Events",
            //    UiHint = "Table",
            //    Icon = pluginPolicy.Icon,
            //    ItemId = "",
            //};
            //entityView.ChildViews.Add(newEntityViewTable);


            //foreach (var sessionView in session.Views)
            //{
            //    newEntityViewTable.ChildViews.Add(
            //        new EntityView
            //        {
            //            ItemId = sessionView.Id,
            //            Icon = pluginPolicy.Icon,
            //            Properties = new List<ViewProperty> {
            //                new ViewProperty {Name = "ItemId", RawValue = sessionView.Id, UiType = "EntityLink" },
            //                new ViewProperty {Name = "Name", RawValue = sessionView.View.Name },
            //                new ViewProperty {Name = "DisplayName", RawValue = sessionView.View.DisplayName, OriginalType = "Html", UiType = "Html" },
            //                new ViewProperty {Name = "Entity", RawValue = sessionView.View.EntityId }
            //            }
            //        }
            //    );
            //}

            return Task.FromResult(entityView);
        }
    }
}
