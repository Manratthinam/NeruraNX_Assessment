using MediatR;
using neuranx.Domain.Entities;

namespace neuranx.Application.Tasks.Commands;

public record CreateTaskCommand(string Title, string Description, string Status, string? AssignedToUserId, string UserId) : IRequest<TaskItem>;
public record UpdateTaskCommand(int Id, string Title, string Description, string Status, string UserId) : IRequest<Unit>;
public record DeleteTaskCommand(int Id, string UserId) : IRequest<Unit>;
