using Mediator;
using Template.Domain.Abstractions.Result;

namespace Template.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
