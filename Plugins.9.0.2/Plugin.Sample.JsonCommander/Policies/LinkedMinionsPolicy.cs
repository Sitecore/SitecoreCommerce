
namespace Plugin.Sample.JsonCommander
{
    using Sitecore.Commerce.Core;

    /// <summary>
    /// PluginPolicy
    /// </summary>
    public class LinkedMinionsPolicy : Policy
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LinkedMinionsPolicy()
        {
            MinionEnvironmentName = "HabitatMinions";
        }

        /// <summary>
        /// Environment for Minions when directly run
        /// </summary>
        public string MinionEnvironmentName { get; set; }


    }
}
