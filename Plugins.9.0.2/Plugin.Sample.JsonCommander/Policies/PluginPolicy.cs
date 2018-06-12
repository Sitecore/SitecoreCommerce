
namespace Plugin.Sample.JsonCommander
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
            Icon = "low_priority";
        }

        /// <summary>
        /// Icon to use for this Plugin in the Sitecore Business Framework
        /// </summary>
        public string Icon { get; set; }


    }
}
