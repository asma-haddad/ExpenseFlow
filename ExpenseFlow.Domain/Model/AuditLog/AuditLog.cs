using ExpenseFlow.Domain.Model.Base;

namespace ExpenseFlow.Domain.Model.AuditLog;

public class AuditLog : BaseModel
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public string IpAddress { get; set; }
    public string UserId { get; set; }
}