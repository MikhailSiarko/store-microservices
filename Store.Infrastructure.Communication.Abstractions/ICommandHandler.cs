using MediatR;

namespace Store.Infrastructure.Communication.Abstractions;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, bool> where TCommand : Command;
