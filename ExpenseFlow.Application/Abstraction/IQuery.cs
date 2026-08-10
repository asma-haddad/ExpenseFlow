using ExpenseFlow.Domain.Base;
using MediatR;

namespace ExpenseFlow.Application.Abstraction
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>> where TResponse : class
    {

    }
}