
using Sitecore.Commerce.Core.Commands;

namespace Plugin.Sample.Promotions.Generator
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.OData.Builder;
	using Sitecore.Commerce.Core;
	using Sitecore.Framework.Conditions;
	using Sitecore.Framework.Pipelines;

	/// <summary>
	/// Defines a block which configures the OData model for the PromotionBook plugin
	/// </summary>
	/// <seealso>
	///     <cref>
	///         Sitecore.Framework.Pipelines.PipelineBlock{Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
	///         Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
	///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
	///     </cref>
	/// </seealso>
	[PipelineDisplayName("SamplePluginConfigureServiceApiBlock")]
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
			Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");

			// Add the entities
			//modelBuilder.AddEntityType(typeof(SampleEntity));

			// Add the entity sets
			//modelBuilder.EntitySet<SampleEntity>("Sample");

			// Add complex types

			// Add unbound functions

			// Add unbound actions
			//var mergeCartsConfiguration = modelBuilder.Action("SampleCommand");
			//mergeCartsConfiguration.Parameter<string>("Id");
			//mergeCartsConfiguration.ReturnsFromEntitySet<CommerceCommand>("Commands");

			return Task.FromResult(modelBuilder);
		}
	}
}
