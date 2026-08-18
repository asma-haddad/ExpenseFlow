using ExpenseFlow.Domain.Base.Language;
using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Model.User;

namespace ExpenseFlow.Domain.Model.Department
{
    public class DepartmentModel : BaseModel
    {
        public Guid ManagerId { get; set; }
        public UserModel Manager { get; set; }

        public LanguagePropertyModel Title { get; set; }
        public LanguagePropertyModel? Description { get; set; }
        public ICollection<UserModel> Employees { get; set; } = new List<UserModel>();


    }
}
