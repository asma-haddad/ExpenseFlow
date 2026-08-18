using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Domain.Model.Expense
{
    public class ExpenseApprovalModel : BaseModel
    {
        public Guid ExpenseId { get; set; }
        public ExpenseModel Expense { get; set; } = null!;
        public LanguagePropertyModel? Comment { get; set; }

        public ApprovalStage ApprovalStage { get; set; }
        public ApprovalDecision ApprovalDecision { get; set; }

    }
}
