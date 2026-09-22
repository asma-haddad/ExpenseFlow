using ExpenseFlow.Application.Abstraction;
using ExpenseFlow.Application.Extensions;
using ExpenseFlow.Domain.Base;
using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static ExpenseFlow.Application.Features.Dashboard.AuditLog.GetAllAuditLog.GetAllAuditLogQuery.Response;

namespace ExpenseFlow.Application.Features.Dashboard.AuditLog.GetAllAuditLog
{
    public class GetAllAuditLogHandler : BaseService, IQueryHandler<GetAllAuditLogQuery.Request, GetAllDataResponse<GetAllAuditLogQuery.Response>>
    {
        public GetAllAuditLogHandler(AppDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

        public async Task<Result<GetAllDataResponse<GetAllAuditLogQuery.Response>>> Handle(GetAllAuditLogQuery.Request request, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllDataResponse<GetAllAuditLogQuery.Response>>();
            var query = context.AuditLog
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.EntityPropertyChanges)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.ServiceName))
            {
                query = query.Where(x => x.ServiceName.Contains(request.ServiceName));
            }

            if (!string.IsNullOrEmpty(request.MethodName))
            {
                query = query.Where(x => x.MethodName.Contains(request.MethodName));
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(x => x.ExecutionTime >= request.StartDate);
            }

            if (request.EndDate.HasValue)
            {
                var endDate = request.EndDate.Value.AddDays(1);
                query = query.Where(x => x.ExecutionTime < endDate);
            }

            if (!string.IsNullOrEmpty(request.AuditLogEventType))
            {
                query = query.Where(x => x.AuditLogEventType.ToString() == request.AuditLogEventType);
            }

            result.Data = await query
                .OrderByDescending(x => x.ExecutionTime)
                .PaginateAsync(log => new GetAllAuditLogQuery.Response
                {
                    Id = log.Id,
                    CorrelationId = log.CorrelationId,
                    AuditLogEventType = log.AuditLogEventType.ToString(),
                    ClientIpAddress = log.ClientIpAddress,
                    ServiceName = log.ServiceName,
                    MethodName = log.MethodName,
                    HttpMethod = log.HttpMethod,
                    RequestUrl = log.RequestUrl,
                    ResponseStatus = log.ResponseStatus,
                    ExecutionTime = log.ExecutionTime,
                    ExecutionDuration = log.ExecutionDuration,
                    EntityName = log.EntityName,
                    Exception = log.Exception,
                    CreatedAt = log.CreatedAt,
                    User = log.User != null ? new UserDto
                    {
                        Id = log.User.Id,
                        FirstName = log.User.FirstName,
                        LastName = log.User.LastName,
                        Email = log.User.Email
                    } : null,
                    PropertyChanges = log.EntityPropertyChanges.Select(pc => new GetAllAuditLogQuery.Response.PropertyChangeDto
                    {
                        PropertyName = pc.PropertyName,
                        PropertyTypeFullName = pc.PropertyTypeFullName,
                        OriginalValue = pc.OriginalValue,
                        NewValue = pc.NewValue
                    }).ToList()
                }, request, cancellationToken);

            return result;
        }
    }

}
