// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEntity.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   SampleEntity model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Messaging
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.OData.Builder;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ManagedLists;

    /// <summary>
    /// SampleDashboardEntity model.
    /// </summary>
    public class MessageEntity : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEntity"/> class.
        /// </summary>
        public MessageEntity()
        {
            this.Components = new List<Component>();
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
            History = new List<HistoryEntryModel>();
            this.Components.Add(new ListMembershipsComponent());
            TargetRoles = new List<string>();
            TargetUsers = new List<string>();
            Models = new List<Model>();
        }

        /// <summary>
        /// List of HistoryEntryModel
        /// </summary>
        public List<HistoryEntryModel> History {get; set;}

        /// <summary>
        /// Target Roles for this message
        /// </summary>
        public List<string> TargetRoles { get; set; }

        /// <summary>
        /// Target Users for this message
        /// </summary>
        public List<string> TargetUsers { get; set; }

        /// <summary>
        /// Models
        /// </summary>
        public List<Model> Models { get; set; }
    }
}