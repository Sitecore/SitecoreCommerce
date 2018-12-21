namespace Plugin.Sample.HandleMissingSitecore.Pipelines.Blocks
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Management;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("MockCommerceTerms")]
    public class MockCommerceTerms : PipelineBlock<CommerceTermsArgument, List<Sitecore.Commerce.Core.LocalizedTerm>, CommercePipelineExecutionContext>
    {
        public override Task<List<Sitecore.Commerce.Core.LocalizedTerm>> Run(CommerceTermsArgument arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(new List<LocalizedTerm>(0));
        }
    }
}
