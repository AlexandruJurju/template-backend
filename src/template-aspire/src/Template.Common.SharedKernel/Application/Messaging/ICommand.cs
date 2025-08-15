using Ardalis.Result;
using MediatR;

namespace Template.Common.SharedKernel.Application.Messaging;

public interface IBaseCommand;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;
