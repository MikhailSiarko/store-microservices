using MediatR;

namespace Store.Infrastructure.Communication.Abstractions;

public abstract class Command : MessageBase, IRequest<bool>;