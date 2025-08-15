using Ardalis.Result;
using MediatR;

namespace Template.Common.SharedKernel.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
