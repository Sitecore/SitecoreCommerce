using Sitecore.Commerce.Core;
using Sitecore.Services.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
