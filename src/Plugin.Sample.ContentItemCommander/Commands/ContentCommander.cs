namespace Plugin.Sample.ContentItemCommander.Commands
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.ContentItemCommander.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Services.Core.Model;

    public class ContentCommander : CommerceCommand
    {
        private static Dictionary<string, ItemModel> _contentItemStash;
        private static Dictionary<string, ContentPathModel> _contentPathStash;

        public ContentCommander(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            if (_contentItemStash == null)
            {
                _contentItemStash = new Dictionary<string, ItemModel>();
            }
            if (_contentPathStash == null)
            {
                _contentPathStash = new Dictionary<string, ContentPathModel>();
            }
        }

        public bool HasStashedItem(CommerceContext commerceContext, string contentItemId)
        {
            if (_contentItemStash.ContainsKey(contentItemId))
            {
                commerceContext.Logger.LogDebug($"=??+ItemStash-HasStashedItem|{contentItemId}|{commerceContext.CurrentLanguage()}|true");
                return true;
            }
            else
            {
                commerceContext.Logger.LogInformation($"=??*ItemStash-HasStashedItem|{contentItemId}|{commerceContext.CurrentLanguage()}|false");
                return false;
            }
        }

        public bool HasStashedPath(CommerceContext commerceContext, string itemPath)
        {
            if (_contentItemStash.ContainsKey(itemPath))
            {
                commerceContext.Logger.LogDebug($"=??+ItemStash-HasStashedPath|{itemPath}|{commerceContext.CurrentLanguage()}|true");
                return true;
            }
            else
            {
                commerceContext.Logger.LogInformation($"=??*ItemStash-HasStashedPath|{itemPath}|{commerceContext.CurrentLanguage()}|false");
                return false;
            }
        }

        public ItemModel GetStashedItem(CommerceContext commerceContext, string contentItemId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                if (_contentItemStash.ContainsKey(contentItemId))
                {
                    var stashedItem = _contentItemStash[contentItemId];
                    return stashedItem;
                }

                commerceContext.Logger.LogInformation($"<<**ItemStash-NotFound|{contentItemId}|{commerceContext.CurrentLanguage()}");
                return null;
            }
        }

        public List<ItemModel> GetStashedPath(CommerceContext commerceContext, string contentItemId)
        {
            var contentPathModel = _contentPathStash[contentItemId];

            return contentPathModel.ItemModels;
        }

        public void StashItem(CommerceContext commerceContext, ItemModel item, string contentItemId)
        {
            lock (_contentItemStash)
            {
                if (!_contentItemStash.ContainsKey(contentItemId))
                {
                    _contentItemStash.Add(contentItemId, item);
                    commerceContext.Logger.LogInformation($"=>>>ItemStash|{contentItemId}");
                }
                else
                {
                    commerceContext.Logger.LogInformation($"=(!)ItemAlreadyStashed|{contentItemId}");
                }
            }
        }

        public void StashPath(CommerceContext commerceContext, string path, List<ItemModel> list)
        {
            lock (_contentPathStash)
            {
                if (!_contentPathStash.ContainsKey(path))
                {
                    var contentPathModel = new ContentPathModel { Name = path, ItemModels = list };

                    _contentPathStash.Add(path, contentPathModel);
                    commerceContext.Logger.LogInformation($"=>>>PathStash|{path}|{list.Count}");
                }
                else
                {
                    commerceContext.Logger.LogInformation($"=>**PathStash-AlreadyStashed|{path}|{list.Count}");
                }
            }
        }
    }
}