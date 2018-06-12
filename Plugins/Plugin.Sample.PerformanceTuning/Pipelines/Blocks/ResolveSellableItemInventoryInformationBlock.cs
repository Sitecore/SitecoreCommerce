// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveSellableItemInventoryInformationBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Plugin.Sample.PerformanceTuning
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the create inventory set block.
    /// </summary>
    [PipelineDisplayName(InventoryConstants.Pipelines.Blocks.ResolveSellableItemInventoryInformationBlock)]
    public class ResolveSellableItemInventoryInformationBlock : PipelineBlock<SellableItem, SellableItem, CommercePipelineExecutionContext>
    {
        private readonly GetCatalogCommand _getCatalogCommand;
        private readonly GetInventoryInformationCommand _getInventoryInformationCommand;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveSellableItemInventoryInformationBlock"/> class.
        /// </summary>
        /// <param name="getCatalogCommand">The get catalog command.</param>
        /// <param name="getInventoryInformationCommand">The get inventory information command.</param>
        public ResolveSellableItemInventoryInformationBlock(
            GetCatalogCommand getCatalogCommand,
            GetInventoryInformationCommand getInventoryInformationCommand)
        {
            this._getCatalogCommand = getCatalogCommand;
            this._getInventoryInformationCommand = getInventoryInformationCommand;
        }

        /// <inheritdoc />
        /// <summary>
        /// Executes the block.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A <see cref="SellableItem" />
        /// </returns>
        public override async Task<SellableItem> Run(SellableItem arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument can not be null");

            var productArgument =
                context.CommerceContext.GetObject<ProductArgument>();

            if (productArgument == null)
            {
                return arg;
            }

            var catalog =
                        context.CommerceContext
                            .GetEntity<Catalog>(x => x.Name.Equals(
                            productArgument.CatalogName,
                            StringComparison.OrdinalIgnoreCase));
            if (catalog == null)
            {
                return arg;
            }

            context.CommerceContext.AddEntity(catalog);

            if (arg.HasPolicy<AvailabilityAlwaysPolicy>())
            {
                //do nothing, digital product
            }
            else
            {
                

                var requestContext = context.CommerceContext.GetObject<RequestContext>();
                if (string.IsNullOrEmpty(requestContext.Path))
                {
                    return arg;
                }
                switch (requestContext.Path)
                {
                    case "/api/SetCartFulfillment":
                        return arg;
                    case "/api/AddCouponToCart":
                        return arg;
                    case "/api/AddFederatedPayment":
                        return arg;
                    //case "/api/CreateOrder":
                    //    return arg;
                    case "/api/AddGiftCardPayment":
                        return arg;
                        //default:
                        //return arg;
                }

                context.Logger.LogInformation($"ResolveSellableItemInventoryInformationBlock.RequestPath={requestContext.Path}");

                var itemVariationsComponent = arg.GetComponent<ItemVariationsComponent>();
                var variant = itemVariationsComponent.GetComponents<ItemVariationComponent>().FirstOrDefault(p => p.Id == productArgument.VariantId);
                if(variant == null)
                {
                    //do nothing
                }
                else
                {
                    if (variant.HasPolicy<AvailabilityAlwaysPolicy>())
                    {
                        //do nothing
                    }
                    else
                    {
                        

                        if (catalog == null)
                        {
                            catalog =
                                await this._getCatalogCommand.Process(
                                    context.CommerceContext,
                                    productArgument.CatalogName);

                            if (catalog == null)
                            {
                                return arg;
                            }

                            context.CommerceContext.AddEntity(catalog);
                        }

                        await this._getInventoryInformationCommand.Process(
                            context.CommerceContext,
                            $"{CommerceEntity.IdPrefix<InventorySet>()}{catalog.DefaultInventorySetName}",
                            arg.Id,
                            productArgument.VariantId,
                            true);
                    }
                }
            }
            

            return arg;
        }
    }
}
