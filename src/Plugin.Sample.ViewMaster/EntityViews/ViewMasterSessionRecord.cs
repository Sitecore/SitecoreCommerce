namespace Plugin.Sample.ViewMaster.EntityViews
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using global::Plugin.Sample.ViewMaster.Entities;

    using Newtonsoft.Json;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.ViewMaster.Policies.PluginPolicy;

    [PipelineDisplayName("ViewMasterSessionRecord")]
    public class ViewMasterSessionRecord : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public ViewMasterSessionRecord(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.EntityId != "ViewMaster_Session")
            {
                return Task.FromResult(entityView);
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var session = entityViewArgument.Entity as ViewMasterSessionEntity;

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

                JsonSerializer serializer = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.All,
                    NullValueHandling = NullValueHandling.Ignore
                };

                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, selectedView.View);
                }

                var entityViewAsJson = sw.ToString();

                var entityViewSeparator = new EntityView
                {
                    Name = "Below is View Recorded",
                    UiHint = "Flat",
                    Icon = pluginPolicy.Icon,
                    ItemId = string.Empty,
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
                    ItemId = string.Empty,
                };
                entityView.ChildViews.Add(entityViewRawJson);

                entityViewAsJson = Regex.Replace(entityViewAsJson, @"}", "}<br>");
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

            return Task.FromResult(entityView);
        }
    }
}
