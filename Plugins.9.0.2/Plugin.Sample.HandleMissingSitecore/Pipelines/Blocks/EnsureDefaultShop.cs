// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateShopCurrencyBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.HandleMissingSitecore
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Shops;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the validate storefront currency block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.Boolean, System.Boolean,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureDefaultShop")]
    public class EnsureDefaultShop : PipelineBlock<string, bool, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// True if the Storefront's currency is valid, false otherwise.
        /// </returns>
        public override async Task<bool> Run(string arg, CommercePipelineExecutionContext context)
        {
            //if (!arg)
            //{
            //    // the call is already invalid so no need to further process
            //    return false;
            //}

            // Check the BasePath of the call
            // We don't validate storefront for CommerceOps calls
            if (context.CommerceContext.GetObjects<RequestContext>().First().BasePath.Equals(context.GetPolicy<KnownServiceRoutesPolicy>().CommerceOps, StringComparison.OrdinalIgnoreCase)
                 || (string.IsNullOrEmpty(context.CommerceContext.CurrentShopName()) && context.CommerceContext.Environment.Name.Equals(context.CommerceContext.GlobalEnvironment.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            var shop = context.CommerceContext.GetObjects<Shop>().FirstOrDefault();
            if (shop == null)
            {
                //return false;
                shop = new Shop {Id = arg, Name = arg, DefaultCurrency = "USD", Currencies = new System.Collections.Generic.List<Currency>() { new Currency("USD") } };
                context.CommerceContext.AddObject(shop);
                return true;

            }

            var currency = context.CommerceContext.CurrentCurrency();
            if (currency.Equals(shop.DefaultCurrency, StringComparison.OrdinalIgnoreCase) || shop.Currencies.Any(p => p.Code.Equals(currency, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            await context.CommerceContext.AddMessage(
                context.GetPolicy<KnownResultCodes>().Error,
                "InvalidShopCurrency",
                new object[] { currency, shop.Name },
                $"Currency '{currency}' for Shop '{shop.Name}' was not found.");

            return false;
        }
    }
}
