namespace Sitecore.Commerce.Plugin.Content.Tests
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Content.Tests.TestUtils;

    using Xunit;

    public class EnsureContentItemChildrenBlockFixture
    {
        private readonly CommercePipelineExecutionContext _context;

        public EnsureContentItemChildrenBlockFixture()
        {
            this._context = TestHelper.GetPipelineExecutionContext();
        }

        [Theory, AutoNSubstituteData]
        public async void Run_NotAContentItem(CommerceEntity entity)
        {
            var commander = Substitute.For<ContentCommander>(Substitute.For<IServiceProvider>());
            var block = new EnsureContentItemChildrenBlock(commander);

            var result = await block.Run(entity, this._context);

            result.Should().Be(entity);
            await commander.DidNotReceiveWithAnyArgs().SetContentItemChildren(null, null);
        }

        [Fact]
        public async void Run_NullEntity()
        {
            var commander = Substitute.For<ContentCommander>(Substitute.For<IServiceProvider>());
            var block = new EnsureContentItemChildrenBlock(commander);

            var result = await block.Run(null, this._context);

            result.Should().BeNull();
            await commander.DidNotReceiveWithAnyArgs().SetContentItemChildren(null, null);
        }

        [Theory, AutoNSubstituteData]
        public async void Run(ContentItem entity)
        {
            var commander = Substitute.For<ContentCommander>(Substitute.For<IServiceProvider>());
            var block = new EnsureContentItemChildrenBlock(commander);

            var result = await block.Run(entity, this._context);

            result.Should().Be(entity);
            await commander.ReceivedWithAnyArgs(1).SetContentItemChildren(null, null);
        }
    }
}
