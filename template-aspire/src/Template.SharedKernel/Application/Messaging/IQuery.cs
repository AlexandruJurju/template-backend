using Ardalis.Result;
using MediatR;

namespace Template.SharedKernel.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
