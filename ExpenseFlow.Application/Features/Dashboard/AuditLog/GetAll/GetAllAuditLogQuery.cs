using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Domain.Base.Dto;

namespace ExpenseFlow.Application.Features.Dashboard.AuditLog.GetAllAuditLog
{
    public class GetAllAuditLogQuery
    {
        public class Request : SearchRequest, IQuery<GetAllDataResponse<Response>>
        {
            public string? ServiceName { get; set; }
            public string? MethodName { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? AuditLogEventType { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public string CorrelationId { get; set; }
            public string AuditLogEventType { get; set; }
            public string ClientIpAddress { get; set; }
            public string ServiceName { get; set; }
            public string MethodName { get; set; }
            public string HttpMethod { get; set; }
            public string RequestUrl { get; set; }
            public string ResponseStatus { get; set; }
            public DateTime? ExecutionTime { get; set; }
            public string ExecutionDuration { get; set; }
            public string EntityName { get; set; }
            public string Exception { get; set; }
            public DateTime CreatedAt { get; set; }

            public UserDto User { get; set; }
            public ICollection<PropertyChangeDto> PropertyChanges { get; set; } = new List<PropertyChangeDto>();

            public class UserDto
            {
                public Guid Id { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string Email { get; set; }
            }

            public class PropertyChangeDto
            {
                public string PropertyName { get; set; }
                public string PropertyTypeFullName { get; set; }
                public string OriginalValue { get; set; }
                public string NewValue { get; set; }
            }

        }
    }

}

