using Ardalis.Result;
using MediatR;

namespace Template.SharedKernel.Application.Messaging;

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse>;
