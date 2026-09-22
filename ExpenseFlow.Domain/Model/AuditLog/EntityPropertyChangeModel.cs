using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.AuditLog
{
    public class EntityPropertyChangeModel : BaseModel

    {
        public string NewValue { get; set; }
        public Guid AuditLogId { get; set; }
        public string PropertyName { get; set; }
        public string OriginalValue { get; set; }
        public string PropertyTypeFullName { get; set; }
    }
}
