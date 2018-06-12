// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetItemsByPathBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.ContentItemCommander
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Management;
    using Sitecore.Framework.Caching;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Services.Core.Model;

    /// <summary>
    ///  Defines a block which gets sitecore items by path.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Management.ItemModelArgument,
    ///         System.Collections.Generic.IEnumerable{Sitecore.Services.Core.Model.ItemModel},
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ManagementConstants.Pipelines.Blocks.GetItemsByPathBlock)]
    public class GetItemsByPathBlock : PipelineBlock<ItemModelArgument, IEnumerable<ItemModel>, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commander;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetItemsByPathBlock"/> class.
        /// </summary>
        /// <param name="commander">The commander.</param>
        public GetItemsByPathBlock(CommerceCommander commander)
        {
            this._commander = commander;
        }

        /// <summary>
        /// The run
        /// </summary>s
        /// <param name="arg">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ItemModel"/>.
        /// </returns>
        public override async Task<IEnumerable<ItemModel>> Run(ItemModelArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: argument cannot be null");
            Condition.Requires(arg.ItemPathOrId).IsNotNullOrEmpty($"{this.Name}: Item path can not be null or empty");

            var language = string.IsNullOrEmpty(arg.Language) ? context.CommerceContext.CurrentLanguage() : arg.Language;
            var cacheKey = $"{arg.ItemPathOrId}|{language}";
            var cachePolicy = context.GetPolicy<ManagementCachePolicy>();
            ICache cache = null;
            List<ItemModel> items;

            //Check Stash
            var contentCommander = this._commander.Command<ContentCommander>();
            if (contentCommander.HasStashedItem(context.CommerceContext, cacheKey))
            {
                return contentCommander.GetStashedPath(context.CommerceContext, cacheKey);
            }

            var semaphore = this._commander.CurrentNodeContext(context.CommerceContext).GetOrAddSemaphore(cacheKey, cachePolicy.ItemsCacheName);

            if (cachePolicy.AllowCaching)
            {
                cache = await this._commander.Pipeline<IGetEnvironmentCachePipeline>().Run(new EnvironmentCacheArgument { CacheName = cachePolicy.ItemsCollectionCacheName }, context);
                items = await cache.Get<List<ItemModel>>(cacheKey);
                if (items != null)
                {
                    return items.AsEnumerable();
                }

                var isFirst = semaphore.CurrentCount == 1;

                await semaphore.WaitAsync(); // Wait at this line.

                if (isFirst == false)
                {
                    // Try again.
                    items = await cache.Get<List<ItemModel>>(cacheKey);
                    if (items != null)
                    {
                        semaphore.Release();
                        return items.AsEnumerable();
                    }
                }
            }

            context.Logger.LogInformation($"{this.Name}.{cacheKey}");

            try
            {
                items = await this.GetItems(context.CommerceContext, arg.ItemPathOrId, language);
                if (items != null)
                {
                    this._commander.Command<ContentCommander>()
                        .StashPath(context.CommerceContext, cacheKey, items);

                    if (cache != null && cachePolicy.AllowCaching)
                    {
                        await cache.Set(cacheKey, new Cachable<List<ItemModel>>(items, 1), cachePolicy.GetCacheEntryOptions());
                    }

                    return items.AsEnumerable();
                }
            }
            catch (Exception e)
            {
                context.CommerceContext.LogExceptionAndMessage(this.Name, e);
            }
            finally
            {
                if (cachePolicy.AllowCaching)
                {
                    semaphore.Release();
                }
            }

            context.Logger.LogError($"{this.Name}: Sitecore Item Service Get item failed, Item {arg.ItemPathOrId} not found.");
            var defaultItems = new List<ItemModel>();
            if (cache != null && cachePolicy.AllowCaching)
            {
                await cache.Set(
                    cacheKey,
                    new Cachable<List<ItemModel>>(defaultItems, 1),
                    cachePolicy.GetCacheEntryOptions());
            }

            return defaultItems;
        }
        
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="path">The items path.</param>
        /// <param name="language">The language.</param>
        /// <returns>A list of <see cref="ItemModel"/></returns>
        protected virtual async Task<List<ItemModel>> GetItems(CommerceContext context, string path, string language = null)
        {
            Condition.Requires(context).IsNotNull($"{this.Name}: context cannot be null.");
            Condition.Requires(path).IsNotNullOrEmpty($"{this.Name}: path cannot be null or empty.");

            //TODO
            var items = await new SitecoreConnectionManager().GetItemsByPathAsync(context, path, language);

            return items;
        }
    }
}
