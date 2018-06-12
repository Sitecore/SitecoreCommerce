
namespace Plugin.Sample.Search.Management
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
            Icon = "find_replace";
            IsDisabled = true;
        }

        /// <summary>
        /// Icon to use for this Plugin in the Sitecore Business Framework
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Plugin is Disabled (true)
        /// </summary>
        public bool IsDisabled { get; set; }
    }
}
