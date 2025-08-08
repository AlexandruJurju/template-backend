using MediatR;
using Template.SharedKernel.Application.CustomResult;

namespace Template.SharedKernel.Application.Messaging;

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse>;
