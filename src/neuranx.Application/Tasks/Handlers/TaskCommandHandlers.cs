using MediatR;
using neuranx.Application.Interfaces;
using neuranx.Domain.Entities;

namespace neuranx.Application.Tasks.Commands;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskItem>
{
    private readonly ITaskRepository _repository;

    public CreateTaskCommandHandler(ITaskRepository repository) { _repository = repository; }

    public async Task<TaskItem> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        return await _repository.AddTaskAsync(new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            AssignedToUserId = !string.IsNullOrEmpty(request.AssignedToUserId) ? Guid.Parse(request.AssignedToUserId) : Guid.Parse(request.UserId),
            UserId = Guid.Parse(request.UserId)
        });
    }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Unit>
{
    private readonly ITaskRepository _repository;

    public UpdateTaskCommandHandler(ITaskRepository repository) { _repository = repository; }

    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetTaskByIdAsync(request.Id, request.UserId);
        if (task == null) throw new KeyNotFoundException($"Task {request.Id} not found");

        task.Tasks.Title = request.Title;
        task.Tasks.Description = request.Description;
        task.Tasks.Status = request.Status;
        await _repository.UpdateTaskAsync(task.Tasks);
        return Unit.Value;
    }
}

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Unit>
{
    private readonly ITaskRepository _repository;

    public DeleteTaskCommandHandler(ITaskRepository repository) { _repository = repository; }

    public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetTaskByIdAsync(request.Id, request.UserId);
        if (task == null) throw new KeyNotFoundException($"Task {request.Id} not found");

        await _repository.DeleteTaskAsync(task.Tasks);
        return Unit.Value;
    }
}
