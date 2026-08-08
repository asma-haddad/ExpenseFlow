using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.User
{
    public class SessionModel : BaseModel
    {

        public string Language { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }

        public Guid? RefId { get; set; }
        public Guid? UserId { get; set; }
        public UserModel User { get; set; }
    }
}