
namespace Plugin.Sample.JsonCommander
{
    using Sitecore.Commerce.Core;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Component to track logical deletion of an Entity.
    /// </summary>
    public class LogicalDeleteComponent : Component
    {
    
        /// <summary>
        /// Constructor
        /// </summary>
        public LogicalDeleteComponent()
        {
            DeleteDate = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Date of Deletion
        /// </summary>
        public DateTimeOffset DeleteDate { get; set; }

    }
}