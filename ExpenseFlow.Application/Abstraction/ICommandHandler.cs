using ExpenseFlow.Domain.Base;
using MediatR;

namespace ExpenseFlow.Application.Abstraction
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result> where TCommand : ICommand
    {
    }

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>> where TCommand : ICommand<TResponse> where TResponse : class
    {

    }
}