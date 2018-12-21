
namespace Plugin.Sample.ListMaster.Policies
{
    using Sitecore.Commerce.Core;

    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            this.Icon = "elements3";
            this.IsDisabled = true;
        }

        public string Icon { get; set; }

        public bool IsDisabled { get; set; }
    }
}
