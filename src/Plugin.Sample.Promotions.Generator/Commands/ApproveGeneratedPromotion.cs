namespace Plugin.Sample.Promotions.Generator.Commands
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Promotions;
    
    public class ApproveGeneratedPromotion : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;
        
        public ApproveGeneratedPromotion(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public async Task<bool> Process(CommerceContext commerceContext, Promotion promotion, List<Policy> policies = null)
        {
            using (CommandActivity.Start(commerceContext, this))
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

                await this._commerceCommander.PersistEntity(commerceContext, createdPromotion);

                return true;
            }
        }
    }
}