using ExpenseFlow.Domain.Base;
using MediatR;

namespace ExpenseFlow.Application.Abstraction
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse> where TResponse : class
    {

    }
}


