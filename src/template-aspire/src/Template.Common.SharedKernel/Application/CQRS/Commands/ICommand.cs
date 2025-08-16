using Ardalis.Result;
using MediatR;

namespace Template.Common.SharedKernel.Application.CQRS.Commands;

public interface IBaseCommand;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;
