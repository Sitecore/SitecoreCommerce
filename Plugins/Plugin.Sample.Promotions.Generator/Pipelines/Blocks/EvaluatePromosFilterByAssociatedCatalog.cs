
using System;

namespace Plugin.Sample.Promotions.Generator
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System.Collections.Generic;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Plugin.Carts;
    using System.Linq;

    /// <summary>
    /// Defines a block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Plugin.Sample.Promotions.Generator.SampleArgument,
    ///         Plugin.Sample.Promotions.Generator.SampleEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EvaluatePromosFilterByAssociatedCatalog")]
	public class EvaluatePromosFilterByAssociatedCatalog : PipelineBlock<IEnumerable<Promotion>, IEnumerable<Promotion>, CommercePipelineExecutionContext>
	{
		/// <summary>
		/// The execute.
		/// </summary>
		/// <param name="arg">
		/// The SampleArgument argument.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		/// <returns>
		/// The <see cref="SampleEntity"/>.
		/// </returns>
		public override Task<IEnumerable<Promotion>> Run(IEnumerable<Promotion> arg, CommercePipelineExecutionContext context)
		{
			Condition.Requires(arg).IsNotNull($"{this.Name}: The argument can not be null");

            var cart = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p=>p.Entity is Cart)?.Entity as Cart;

            var lastBlockResult = context.LastBlockResult;

            if (cart != null)
            {
                cart.GetComponent<MessagesComponent>().AddMessage(
                                context.GetPolicy<KnownMessageCodePolicy>().Promotions,
                                $"EvaluatePromosFilterByAssociatedCatalog.PromotionsRemaining: {arg.Count()}");
            }

            //var result = await Task.Run(() => new SampleEntity() { Id = Guid.NewGuid().ToString()});
			return Task.FromResult(arg);
		}
	}
}
