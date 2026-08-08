using MediatR;

namespace ExpenseFlow.Application.Abstraction
{
    public interface ICommand : IRequest
    {
    }
    public interface ICommand<TResponse> : IRequest<TResponse>
    {
    }
}
