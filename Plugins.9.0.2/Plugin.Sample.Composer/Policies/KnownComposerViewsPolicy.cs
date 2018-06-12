// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownComposerViewsPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// Defines the known composer views policy.
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class KnownComposerViewsPolicy : Policy
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Sample.Composer.KnownComposerViewsPolicy" /> class.
        /// </summary>
        public KnownComposerViewsPolicy()
        {
            this.ComposerDashboard = "ComposerDashboard";
            this.ComposerTemplates = "ComposerTemplates";

            this.AddChildView = "AddChildView";
            this.AddChildViewFromTemplate = "AddChildViewFromTemplate";

            this.EditView = "EditView";
            this.MakeTemplate = "MakeTemplate";

            this.AddProperty = "AddProperty";
            this.RemoveProperty = "RemoveProperty";
            this.AddMinMaxPropertyConstrain = "AddMinMaxPropertyConstraint";
            this.AddSelectionOptionPropertyConstrain = "AddSelectionOptionPropertyConstraint";
        }

        /// <summary>
        /// Gets or sets the composer dashboard.
        /// </summary>
        /// <value>
        /// The composer dashboard.
        /// </value>
        public string ComposerDashboard { get; set; }

        /// <summary>
        /// Gets or sets the composer templates.
        /// </summary>
        /// <value>
        /// The composer templates.
        /// </value>
        public string ComposerTemplates { get; set; }
        
        /// <summary>
        /// Gets or sets the add child view.
        /// </summary>
        /// <value>
        /// The add child view.
        /// </value>
        public string AddChildView { get; set; }

        /// <summary>
        /// Gets or sets the add child view from template.
        /// </summary>
        /// <value>
        /// The add child view from template.
        /// </value>
        public string AddChildViewFromTemplate { get; set; }

        /// <summary>
        /// Gets or sets the edit view.
        /// </summary>
        /// <value>
        /// The edit view.
        /// </value>
        public string EditView { get; set; }

        /// <summary>
        /// Gets or sets the make template.
        /// </summary>
        /// <value>
        /// The make template.
        /// </value>
        public string MakeTemplate { get; set; }

        /// <summary>
        /// Gets or sets the add property.
        /// </summary>
        /// <value>
        /// The add property.
        /// </value>
        public string AddProperty { get; set; }

        /// <summary>
        /// Gets or sets the remove property.
        /// </summary>
        /// <value>
        /// The remove property.
        /// </value>
        public string RemoveProperty { get; set; }

        /// <summary>
        /// Gets or sets the add minimum maximum property constrain.
        /// </summary>
        /// <value>
        /// The add minimum maximum property constrain.
        /// </value>
        public string AddMinMaxPropertyConstrain { get; set; }

        /// <summary>
        /// Gets or sets the add selection option property constrain.
        /// </summary>
        /// <value>
        /// The add selection option property constrain.
        /// </value>
        public string AddSelectionOptionPropertyConstrain { get; set; }
    }
}
