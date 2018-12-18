
namespace Plugin.Sample.Search.Management.Policies
{
    using Sitecore.Commerce.Core;

    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            this.Icon = "find_replace";
            this.IsDisabled = true;
        }
        
        public string Icon { get; set; }
        
        public bool IsDisabled { get; set; }
    }
}
