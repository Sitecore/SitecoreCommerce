// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   SampleEntity model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.OData.Builder;
    using Sitecore.Commerce.Core;

    /// <summary>
    /// AppService model.
    /// </summary>
    public class AppService : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppService"/> class.
        /// </summary>
        public AppService()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppService"/> class. 
        /// Public Constructor
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public AppService(string id): this()
        {
            this.Id = id;
        }

        /// <summary>
        /// Service Type
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }


        ///// <summary>
        ///// Gets or sets the list of child components in the SampleEntity
        ///// </summary>
        //[Contained]
        //public IEnumerable<SampleComponent> ChildComponents { get; set; }
    }
}