
namespace Plugin.Sample.Plugin.Enhancements.Policies
{
    using Sitecore.Commerce.Core;

    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            this.Icon = "cubes";
        }

        public string Icon { get; set; }
    }
}
