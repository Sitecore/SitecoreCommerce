
namespace Plugin.Sample.Roles.Enhancements
{
    using Sitecore.Commerce.Core;
    
    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            Icon = "cubes";
            IsDisabled = true;
        }
        
        public string Icon { get; set; }
        
        public bool IsDisabled { get; set; }
    }
}
