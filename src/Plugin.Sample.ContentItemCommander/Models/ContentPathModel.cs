using Sitecore.Commerce.Core;
using Sitecore.Services.Core.Model;

using System.Collections.Generic;

namespace Plugin.Sample.ContentItemCommander.Models
{
    public class ContentPathModel : Model
    {
        public ContentPathModel()
        {
            ItemModels = new List<ItemModel>();
        }

        public List<ItemModel> ItemModels { get; set; }
    }
}
