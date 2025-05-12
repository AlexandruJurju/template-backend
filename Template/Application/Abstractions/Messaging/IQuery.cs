using Domain.Abstractions.Result;
using Mediator;

namespace Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
