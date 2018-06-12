// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewMasterViewRecord.cs" company="Sitecore Corporation">
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
    public class ViewMasterViewRecord : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewmasterSessionEntity"/> class.
        /// </summary>
        public ViewMasterViewRecord()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
            View = new EntityView();
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMasterViewRecord"/> class. 
        /// Public Constructor
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public ViewMasterViewRecord(string id): this()
        {
            this.Id = id;
        }

        /// <summary>
        /// Primary View
        /// </summary>
        public EntityView View { get; set; }

        /// <summary>
        /// List of Headers
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Function to run if NotNull
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool NotNull(Func<bool> func)
        {
            if (this == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}