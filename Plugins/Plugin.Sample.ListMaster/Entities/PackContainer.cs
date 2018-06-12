// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackContainer.cs" company="Sitecore Corporation">
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
    using Sitecore.Commerce.Plugin.ManagedLists;

    /// <summary>
    /// PackContainer model.
    /// </summary>
    public class PackContainer : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackContainer"/> class.
        /// </summary>
        public PackContainer()
        {
            //this.Components = new List<Component>();
            Entities = new List<CommerceEntity>();
            ManagedLists = new List<ManagedList>();
        }

        ///// <summary>
        ///// Gets or sets the list of child components in the SampleEntity
        ///// </summary>
        //[Contained]
        //public IEnumerable<SampleComponent> ChildComponents { get; set; }

            /// <summary>
            /// List of Entities
            /// </summary>
        public List<CommerceEntity> Entities { get; set; }

        /// <summary>
        /// List of Managed Lists
        /// </summary>
        public List<ManagedList> ManagedLists { get; set; }

    }
}