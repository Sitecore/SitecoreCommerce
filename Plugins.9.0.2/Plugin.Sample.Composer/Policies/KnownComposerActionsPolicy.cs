// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownComposerActionsPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// Defines the known composer actions policy.
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class KnownComposerActionsPolicy : Policy
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Sample.Composer.KnownComposerActionsPolicy" /> class.
        /// </summary>
        public KnownComposerActionsPolicy()
        {
            this.EnsureDefaultTemplates = "EnsureDefaultTemplates";
            this.ClearTemplates = "ClearTemplates";
            this.RemoveTemplate = "RemoveTemplate";

            this.AddChildView = "AddChildView";
            this.AddChildViewFromTemplate = "AddChildViewFromTemplate";
            
            this.EditView = "EditView";
            this.RemoveView = "RemoveView";
            this.MakeTemplate = "MakeTemplate";

            this.AddProperty = "AddProperty";
            this.RemoveProperty = "RemoveProperty";
            this.AddMinMaxPropertyConstraint = "AddMinMaxPropertyConstraint";
            this.AddSelectionOptionPropertyConstraint = "AddSelectionOptionPropertyConstraint";
        }
        
        /// <summary>
        /// Gets or sets the composer ensure default templates.
        /// </summary>
        /// <value>
        /// The composer ensure default templates.
        /// </value>
        public string EnsureDefaultTemplates { get; set; }

        /// <summary>
        /// Gets or sets the composer clear templates.
        /// </summary>
        /// <value>
        /// The composer clear templates.
        /// </value>
        public string ClearTemplates { get; set; }

        /// <summary>
        /// Gets or sets the remove template.
        /// </summary>
        /// <value>
        /// The remove template.
        /// </value>
        public string RemoveTemplate { get; set; }

        /// <summary>
        /// Gets or sets the composer add child view from template.
        /// </summary>
        /// <value>
        /// The composer add child view from template.
        /// </value>
        public string AddChildViewFromTemplate { get; set; }

        /// <summary>
        /// Gets or sets the composer add child view.
        /// </summary>
        /// <value>
        /// The composer add child view.
        /// </value>
        public string AddChildView { get; set; }

        /// <summary>
        /// Gets or sets the composer edit view.
        /// </summary>
        /// <value>
        /// The composer edit view.
        /// </value>
        public string EditView { get; set; }

        /// <summary>
        /// Gets or sets the remove view.
        /// </summary>
        /// <value>
        /// The remove view.
        /// </value>
        public string RemoveView { get; set; }

        /// <summary>
        /// Gets or sets the make template.
        /// </summary>
        /// <value>
        /// The make template.
        /// </value>
        public string MakeTemplate { get; set; }

        /// <summary>
        /// Gets or sets the composer add property.
        /// </summary>
        /// <value>
        /// The composer add property.
        /// </value>
        public string AddProperty { get; set; }

        /// <summary>
        /// Gets or sets the composer remove property.
        /// </summary>
        /// <value>
        /// The composer remove property.
        /// </value>
        public string RemoveProperty { get; set; }

        /// <summary>
        /// Gets or sets the add minimum maximum property constraint.
        /// </summary>
        /// <value>
        /// The add minimum maximum property constrain.
        /// </value>
        public string AddMinMaxPropertyConstraint { get; set; }

        /// <summary>
        /// Gets or sets the add selection option property constraint.
        /// </summary>
        /// <value>
        /// The add selection option property constrain.
        /// </value>
        public string AddSelectionOptionPropertyConstraint { get; set; }
    }
}
