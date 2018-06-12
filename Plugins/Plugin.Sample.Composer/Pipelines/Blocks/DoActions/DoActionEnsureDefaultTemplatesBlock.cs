// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionEnsureDefaultTemplatesBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Views;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the do action ensure default templates block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.DoActionEnsureDefaultTemplatesBlock)]
    public class DoActionEnsureDefaultTemplatesBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionEnsureDefaultTemplatesBlock"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionEnsureDefaultTemplatesBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="entityView">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (string.IsNullOrEmpty(entityView?.Action)
                || !entityView.Action.Equals(context.GetPolicy<KnownComposerActionsPolicy>().EnsureDefaultTemplates, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            await this.AddBaseNaming(context.CommerceContext);
            await this.FormAddChildView(context.CommerceContext);

            return entityView;
        }

        /// <summary>
        /// Add base naming
        /// </summary>
        /// <param name="commerceContext">The commerce context</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task AddBaseNaming(CommerceContext commerceContext)
        {
            var name = "BaseNaming";
            var displayName = "Base Naming";

            var newTemplate = new ComposerTemplate($"{CommerceEntity.IdPrefix<ComposerTemplate>()}{name}") { Name = name, DisplayName = displayName };
            newTemplate.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<ComposerTemplate>());

            var newEntityView = newTemplate.GetComponent<EntityViewComponent>().View;
            newEntityView.Name = name;
            newEntityView.DisplayName = displayName;
            newEntityView.Icon = "piece";

            newEntityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Name",
                    DisplayName = "Name",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty,
                    OriginalType = "System.String"
                });

            newEntityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DisplayName",
                    DisplayName = "Display Name",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty,
                    OriginalType = "System.String"
                });

            await this._commerceCommander.PersistEntity(commerceContext, newTemplate);
        }

        /// <summary>
        /// Form add child view
        /// </summary>
        /// <param name="commerceContext">The commerce context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task FormAddChildView(CommerceContext commerceContext)
        {
            var name = "AddChildView";
            var displayName = "AddChildView";

            var newTemplate = new ComposerTemplate($"{CommerceEntity.IdPrefix<ComposerTemplate>()}{name}") { Name = name, DisplayName = displayName };
            newTemplate.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<ComposerTemplate>());

            var newEntityView = newTemplate.GetComponent<EntityViewComponent>().View;
            newEntityView.Name = name;
            newEntityView.DisplayName = displayName;
            newEntityView.Icon = "piece";

            newEntityView.Properties.Add(
                new ViewProperty
                {
                    Name = "BaseNaming",
                    DisplayName = "Base Naming",
                    RawValue = "BaseNaming",
                    OriginalType = "Template"
                });

            newEntityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Name",
                    DisplayName = "Name",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty,
                    OriginalType = "System.String"
                });

            newEntityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DisplayName",
                    DisplayName = "Display Name",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty,
                    OriginalType = "System.String"
                });

            await this._commerceCommander.PersistEntity(commerceContext, newTemplate);
        }
    }
}
