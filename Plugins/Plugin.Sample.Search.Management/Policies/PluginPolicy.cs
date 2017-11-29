
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
            Icon = "search";
            IsEnabled = false;
        }

        /// <summary>
        /// Indicates Plugin is Enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Icon to use for this Plugin in the Sitecore Business Framework
        /// </summary>
        public string Icon { get; set; }


    }
}
