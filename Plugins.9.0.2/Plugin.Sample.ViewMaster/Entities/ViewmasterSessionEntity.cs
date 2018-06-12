// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewmasterSessionEntity.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   SampleEntity model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.ViewMaster
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.OData.Builder;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// SampleDashboardEntity model.
    /// </summary>
    public class ViewmasterSessionEntity : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewmasterSessionEntity"/> class.
        /// </summary>
        public ViewmasterSessionEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
            Views = new List<ViewMasterViewRecord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewmasterSessionEntity"/> class. 
        /// Public Constructor
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public ViewmasterSessionEntity(string id): this()
        {
            this.Id = id;
        }

        /// <summary>
        /// Views List
        /// </summary>
        public List<ViewMasterViewRecord> Views { get; set; }
    }
}