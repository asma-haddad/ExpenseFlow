using ExpenseFlow.Domain.Model.Base;
using ExpenseFlow.Domain.Model.User;
using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Domain.Model.AuditLog;

public class AuditLog : BaseModel
{
    public string CorrelationId { get; set; }
    public EventType AuditLogEventType { get; set; }

    public string UserRole { get; set; }
    public string SessionId { get; set; }
    public string ClientIpAddress { get; set; }
    public string BrowserInfo { get; set; }
    public string MachineName { get; set; }
    public string MachineVersion { get; set; }
    public string MachineOsVersion { get; set; }
    public string ServiceName { get; set; }
    public string MethodName { get; set; }
    public string HttpMethod { get; set; }
    public string RequestUrl { get; set; }
    public string ResponseStatus { get; set; }
    public DateTime? ExecutionTime { get; set; }
    public string ExecutionDuration { get; set; }
    public string RequestHeaders { get; set; }
    public string QueryParameters { get; set; }
    public string BodyParameters { get; set; }
    public string Exception { get; set; }

    public Guid? EntityId { get; set; }
    public string EntityName { get; set; }

    public Guid UserId { get; set; }
    public UserModel User { get; set; }

    public Guid? AuditScopeId { get; set; }
    public AuditScope AuditScope { get; set; }

    public ICollection<EntityPropertyChangeModel> EntityPropertyChanges { get; set; } = new List<EntityPropertyChangeModel>();
}