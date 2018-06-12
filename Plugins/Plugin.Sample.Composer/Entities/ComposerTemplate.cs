// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerTemplate.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// Defines the composer template entity
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceEntity" />
    public class ComposerTemplate : CommerceEntity
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Sample.Composer.Entities.ComposerTemplate" /> class.
        /// </summary>
        public ComposerTemplate()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
            this.Tags = new List<Tag>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Sample.Composer.Entities.ComposerTemplate" /> class. 
        /// Public Constructor
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public ComposerTemplate(string id) : this()
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>       
        public IList<Tag> Tags { get; set; }

    }
}