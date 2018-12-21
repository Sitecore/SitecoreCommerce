namespace Plugin.Sample.Plugin.Enhancements.Commands
{
    using System.Threading.Tasks;

    using global::Plugin.Sample.Enhancements.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class PluginCommander : CommerceCommand
    {
        public async Task<UserPluginOptions> CurrentUserSettings(CommerceContext commerceContext, CommerceCommander commerceCommander)
        {
            var userPluginOptionsId = $"Entity-UserPluginOptions-{commerceContext.CurrentCsrId().Replace("\\", "|")}";

            var userPluginOptions = await commerceCommander
                    .GetEntity<UserPluginOptions>(commerceContext, userPluginOptionsId)
                ?? new UserPluginOptions
                {
                    Id = userPluginOptionsId
                };

            return userPluginOptions;
        }
    }
}