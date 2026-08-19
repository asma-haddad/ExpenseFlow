using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Model.Category;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Domain.Model.Expense
{
    public class ExpenseModel : BaseModel
    {
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
        public Guid CategoryId { get; set; }
        public CategoryModel Category { get; set; }

        public double Amount { get; set; } = 0;
        public string? ReceiptImageUrl { get; set; } = null;
        public LanguagePropertyModel Title { get; set; }
        public LanguagePropertyModel Description { get; set; }


        public ExpenseStatus ExpenseStatus { get; set; } = ExpenseStatus.PendingManagerApproval;
        public ICollection<ExpenseApprovalModel> ExpenseApprovals { get; set; } = new List<ExpenseApprovalModel>();
    }
}
