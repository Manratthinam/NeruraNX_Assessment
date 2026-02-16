using MediatR;
using neuranx.Application.Common.Models;
using neuranx.Application.Interfaces;
using neuranx.Application.Tasks.Queries;
using neuranx.Domain.Entities;

namespace neuranx.Application.Tasks.Handlers;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PaginatedResult<TaskList>>
{
    private readonly ITaskRepository _repository;

    public GetTasksQueryHandler(ITaskRepository repository) { _repository = repository; }

    public async Task<PaginatedResult<TaskList>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTasksByUserIdAsync(request.UserId, request.PageNumber, request.PageSize);
    }
}

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskList?>
{
    private readonly ITaskRepository _repository;

    public GetTaskByIdQueryHandler(ITaskRepository repository) { _repository = repository; }

    public async Task<TaskList?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetTaskByIdAsync(request.Id, request.UserId);
    }
}

public class GetTasksQuerysHandler : IRequestHandler<GetTasksQuerys, PaginatedResult<TaskList>>
{
    private readonly ITaskRepository _repository;

    public GetTasksQuerysHandler(ITaskRepository repository) { _repository = repository; }

    public async Task<PaginatedResult<TaskList>> Handle(GetTasksQuerys request, CancellationToken cancellationToken)
    {
        return await _repository.GetTasksByAsync(request.PageNumber, request.PageSize);
    }
}