
using MediatR;
using Template.SharedKernel.Result;

namespace Template.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
