// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureServiceApiBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Plugin.SxaRepo
{
    using System.Threading.Tasks;

    using Core;
    using Core.Commands;

    using Microsoft.AspNetCore.OData.Builder;
    using Sitecore.Commerce.XA.Feature.Cart.Models.JsonResults;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which configures the OData model for the Carts plugin
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Plugin.SxaRepo.ConfigureServiceApiBlock")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="modelBuilder">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ODataConventionModelBuilder"/>.
        /// </returns>
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull("The argument can not be null");

            // Add the entity
            modelBuilder.AddEntityType(typeof(CartJsonResult));

            // Add Complex Types
            //modelBuilder.AddComplexType(typeof(Totals));
            //modelBuilder.AddComplexType(typeof(LineAdded));
            //modelBuilder.AddComplexType(typeof(LineUpdated));

            // Add policies
            //modelBuilder.AddComplexType(typeof(LineQuantityPolicy));

            // Add the entity set
            modelBuilder.EntitySet<CartJsonResult>("JsonResults");

            // Add unbound actions
            var mergeCartsConfiguration = modelBuilder.Action("GetCartJson");
            mergeCartsConfiguration.Parameter<string>("Id");
            mergeCartsConfiguration.ReturnsFromEntitySet<CartJsonResult>("JsonResults");

            // Add bound actions

            return Task.FromResult(modelBuilder);
        }
    }
}
