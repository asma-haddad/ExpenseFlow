using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Model.Expense;

namespace ExpenseFlow.Domain.Model.NewFolder
{
    public class CategoryModel : BaseModel
    {
        public LanguagePropertyModel Title { get; set; }
        public ICollection<ExpenseModel> Expenses { get; set; } = new List<ExpenseModel>();

    }
}
