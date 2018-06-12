// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   PluginPolicy policy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using Sitecore.Commerce.Core;

    /// <summary>
    /// PluginPolicy
    /// </summary>
    public class PluginPolicy : Policy
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PluginPolicy()
        {
            Icon = "plug_usb";
        }

        /// <summary>
        /// Icon to use for this Plugin in the Sitecore Business Framework
        /// </summary>
        public string Icon { get; set; }


    }
}
