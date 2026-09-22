using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.AuditLog
{
    public class AuditScope : BaseModel
    {
        public List<AuditLog> Logs { get; } = new();

    }
}
