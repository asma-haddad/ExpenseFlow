using ExpenseFlow.Domain.Base;
using MediatR;

namespace ExpenseFlow.Application.Abstraction
{
    public interface ICommand : IRequest<Result>
    {

    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>> where TResponse : class
    {

    }
}