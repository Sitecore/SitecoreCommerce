namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Plugin.Sample.BizFx.DevOps.Entities;
    using Plugin.Sample.JsonCommander.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("FormPullEnvironment")]
    public class FormPullEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormPullEnvironment(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

            if (entityView.Name != "DevOps-PullEnvironment")
            {
                return entityView;
            }

            var entityViewArgument = _commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            if (entityViewArgument.Entity == null)
            {
                var appService = await _commerceCommander
                    .GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

                if (appService == null)
                {
                }
                else
                {
                    var serviceUri = $"http://{appService.Host}/commerceops/Environments";

                    try
                    {
                        var jsonResponse = await _commerceCommander.Command<JsonCommander>()
                            .Process(context.CommerceContext, serviceUri);

                        dynamic dynJson = JsonConvert.DeserializeObject(jsonResponse.Json);

                        var environments = dynJson.value;

                        var templateViewProperty = new ViewProperty
                        {
                            Name = "Environment",
                            DisplayName = "Selected Environment",
                            IsHidden = false,
                            IsRequired = true,
                            RawValue = string.Empty
                        };

                        entityView.Properties.Add(templateViewProperty);

                        var availableSelections = templateViewProperty.GetPolicy<AvailableSelectionsPolicy>();

                        foreach (var environment in environments)
                        {
                            availableSelections.List.Add(new Selection { Name = environment.Name, DisplayName = environment.Name, IsDefault = false });
                        }

                        entityView.Properties.Add(new ViewProperty
                        {
                            Name = "NameAs",
                            DisplayName = "Name Environment As",
                            IsHidden = false,
                            IsRequired = false,
                            RawValue = string.Empty
                        });
                    }
                    catch (Exception ex)
                    {
                        context.Logger.LogError($"DevOps.FormPullEnvironment.Exception: Message={ex.Message}");
                    }
                }
            }
            return entityView;
        }
    }
}
