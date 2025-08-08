using MediatR;
using Template.SharedKernel.Application.CustomResult;

namespace Template.SharedKernel.Application.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;
