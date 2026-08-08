using MediatR;

namespace ExpenseFlow.Application.Abstraction
{
    public interface IQueryHandler<TQuery, TResponse>
      : IRequestHandler<TQuery, TResponse>
      where TQuery : IQuery<TResponse>
    {
    }
}
