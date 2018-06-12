
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Plugin.Sample.Promotions.Generator
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Promotions;

    /// <summary>
    /// Defines the ApproveGeneratedPromotion command.
    /// </summary>
    public class ApproveGeneratedPromotion : CommerceCommand
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApproveGeneratedPromotion"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ApproveGeneratedPromotion(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="promotion">
        /// The promotion
        /// </param>
        /// <param name="policies">
        /// The policies for the command
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public async Task<bool> Process(CommerceContext commerceContext, Promotion promotion, List<Policy> policies = null)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var createdPromotion = promotion;

               commerceContext.GetMessages().Add(new CommandMessage { Code = "Information", CommerceTermKey = "PromotionBeingApproved", Text = "Promotion Being Approved!" });

                var requestApprovalResponse = await this._commerceCommander.Command<SetApprovalStatusCommand>()
                    .Process(commerceContext.GetPipelineContextOptions().CommerceContext, createdPromotion, 
                    commerceContext.GetPolicy<ApprovalStatusPolicy>().ReadyForApproval, "Generated");

                if (!requestApprovalResponse)
                {
                    commerceContext.Logger.LogWarning($"DoActionGeneratePromotionBook.ApprovalRequestDenied: PromotionId={createdPromotion.Id}");
                }

                createdPromotion = await this._commerceCommander.GetEntity<Promotion>(commerceContext, $"{createdPromotion.Id}");

                var approvalResponse = await this._commerceCommander.Command<SetApprovalStatusCommand>()
                    .Process(commerceContext.GetPipelineContextOptions().CommerceContext, createdPromotion, commerceContext.GetPolicy<ApprovalStatusPolicy>().Approved, "Generated");

                if (!approvalResponse)
                {
                    commerceContext.Logger.LogWarning($"DoActionGeneratePromotionBook.ApprovalRequestDenied: PromotionId={createdPromotion.Id}");
                }
                else
                {
                    commerceContext.GetMessages().Add(new CommandMessage { Code = "Information", CommerceTermKey = "PromotionApproved", Text = "Promotion Approved!" });
                }

                createdPromotion = await this._commerceCommander.GetEntity<Promotion>(commerceContext, $"{createdPromotion.Id}");

                var persistResult = await this._commerceCommander.PersistEntity(commerceContext, createdPromotion);

                return true;
            }
        }
    }
}