using MediatR;
using Template.SharedKernel.Application.CustomResult;

namespace Template.SharedKernel.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
