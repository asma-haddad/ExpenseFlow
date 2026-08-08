using MediatR;

namespace ExpenseFlow.Application.Abstraction
{
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }



}
