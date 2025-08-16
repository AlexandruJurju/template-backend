using Ardalis.Result;
using MediatR;

namespace Template.Common.SharedKernel.Application.CQRS.Queries;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
