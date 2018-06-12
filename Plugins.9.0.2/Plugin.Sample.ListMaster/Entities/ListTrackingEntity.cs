// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleDashboardEntity.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   SampleEntity model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.ListMaster
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.OData.Builder;
    using Sitecore.Commerce.Core;

    /// <summary>
    /// SampleDashboardEntity model.
    /// </summary>
    public class ListTrackingEntity : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListTrackingEntity"/> class.
        /// </summary>
        public ListTrackingEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListTrackingEntity"/> class. 
        /// Public Constructor
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public ListTrackingEntity(string id): this()
        {
            this.Id = id;
            Lists = new List<TrackedList>();
        }

        /// <summary>
        /// List of TrackedList
        /// </summary>
        public List<TrackedList> Lists { get; set; }
    }
}