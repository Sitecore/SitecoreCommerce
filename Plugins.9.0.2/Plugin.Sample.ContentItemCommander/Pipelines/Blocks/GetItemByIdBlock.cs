// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetItemByIdBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.ContentItemCommander
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Management;
    using Sitecore.Framework.Caching;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Services.Core.Model;

    /// <summary>
    /// Defines a block which gets a sitecore item by id.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Management.ItemModelArgument,
    ///         Sitecore.Services.Core.Model.ItemModel, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ManagementConstants.Pipelines.Blocks.GetItemByIdBlock)]
    public class GetItemByIdBlock : PipelineBlock<ItemModelArgument, ItemModel, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commander;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetItemByIdBlock"/> class.
        /// </summary>
        /// <param name="commander">The commander.</param>
        public GetItemByIdBlock(CommerceCommander commander)
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
        public override async Task<ItemModel> Run(ItemModelArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: argument cannot be null");
            Condition.Requires(arg.ItemPathOrId).IsNotNullOrEmpty($"{this.Name}: Item id cannot be null or empty");
            
            var language = string.IsNullOrEmpty(arg.Language) ? context.CommerceContext.CurrentLanguage() : arg.Language;
            var cacheKey = $"{arg.ItemPathOrId}|{language}";
            var cachePolicy = context.GetPolicy<ManagementCachePolicy>();
            var semaphore = this._commander.CurrentNodeContext(context.CommerceContext).GetOrAddSemaphore(cacheKey, cachePolicy.ItemsCacheName);

            try
            {
                ItemModel item;
                ICache cache = null;

                //Add Stashing
                if (this._commander.Command<ContentCommander>().HasStashedItem(context.CommerceContext, cacheKey))
                {
                    return this._commander.Command<ContentCommander>()
                        .GetStashedItem(context.CommerceContext, cacheKey);
                }
                

                if (cachePolicy.AllowCaching)
                {
                    cache = await this._commander.Pipeline<IGetEnvironmentCachePipeline>().Run(new EnvironmentCacheArgument { CacheName = cachePolicy.ItemsCacheName }, context);
                    item = await cache.Get<ItemModel>(cacheKey);
                    if (item != null)
                    {
                        return item;
                    }

                    var isFirst = semaphore.CurrentCount == 1;
                    await semaphore.WaitAsync(); // Wait at this line.
                    if (isFirst == false)
                    {
                        // Try again.
                        item = await cache.Get<ItemModel>(cacheKey);
                        if (item != null)
                        {
                            semaphore.Release();
                            return item;
                        }
                    }
                }

                try
                {
                    item = this.GetItem(context.CommerceContext, arg.ItemPathOrId, language);
                    if (item != null)
                    {
                        this._commander.Command<ContentCommander>()
                            .StashItem(context.CommerceContext, item, cacheKey);

                        if (cache != null && cachePolicy.AllowCaching)
                        {
                            await cache.Set(cacheKey, new Cachable<ItemModel>(item, 1), cachePolicy.GetCacheEntryOptions());
                        }

                        return item;
                    }
                }
                catch (Exception e)
                {
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        e.GetType().Name,
                        new object[] { arg, e },
                        $"{this.Name}: Entity '{arg.ItemPathOrId}' was not found.");
                    return null;
                }
                finally
                {
                    if (cachePolicy.AllowCaching)
                    {
                        semaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                context.CommerceContext.LogException(this.Name, ex);
            }

            context.Logger.LogError($"{this.Name}: Sitecore Item Service Get item failed, Item {arg.ItemPathOrId} not found.");
            return null;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="language">The language.</param>
        /// <returns>A <see cref="ItemModel"/> by id</returns>
        protected virtual ItemModel GetItem(CommerceContext context, string id, string language = null)
        {
            Condition.Requires(context).IsNotNull($"{this.Name}: the context cannot be null.");
            Condition.Requires(id).IsNotNullOrEmpty($"{this.Name}: the id cannot be null or empty.");

            context.Logger.LogInformation($"{this.Name}.{id}: Language={language}");

            ItemModel item = null;
            try
            {
                var startTime = DateTimeOffset.UtcNow;
                var timer = System.Diagnostics.Stopwatch.StartNew();
                item = SitecoreConnectionManager.GetItemById(context, id, language);
                timer.Stop();

                context.TelemetryClient.TrackDependency("Sitecore", "GetItemById", startTime, timer.Elapsed, true);
            }
            catch (Exception ex)
            {
                context.LogException(this.Name, ex);
            }

            return item;
        }
    }
}
