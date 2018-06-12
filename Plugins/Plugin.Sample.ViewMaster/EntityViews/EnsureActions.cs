
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.ViewMaster
{
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureActions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureActions(CommerceCommander commerceCommander)
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

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityView.Name != "ViewMaster")
            {
                return entityView;
            }

            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();


            var userPluginOptions = await this._commerceCommander.Command<Plugin.Enhancements.PluginCommander>().CurrentUserSettings(context.CommerceContext, this._commerceCommander);


            var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Views Recorded");

            if (tableEntityView != null)
            {
                var tableEntityViewActionsPolicy = tableEntityView.GetPolicy<ActionsPolicy>();

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ViewMaster-ClearEvents",
                    DisplayName = "Clear All Events",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    Icon = "delete"
                });



                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ViewMaster-StartRecording",
                    DisplayName = "Start Recording",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    Icon = "delete"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ViewMaster-StopRecording",
                    DisplayName = "Stop Recording",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    Icon = "delete"
                });

            }

            return entityView;
        }
    }
}
