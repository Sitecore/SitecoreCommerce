namespace Plugin.Sample.Enhancements.Entities
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    
    public class UserPluginOptions : CommerceEntity
    {
        public UserPluginOptions()
        {
            this.EnabledPlugins = new List<string>();
        }

        public List<string> EnabledPlugins { get; set; }

        public Task<bool> Initialize(CommerceContext commerceContext)
        {
            this.Id = $"Entity-UserPluginOptions-{commerceContext.CurrentCsrId().Replace("\\", "|")}";
            
            return Task.FromResult(true);
        }
    }
}