using MediatR;
using neuranx.Application.Common.Models;
using neuranx.Domain.Entities;

namespace neuranx.Application.Tasks.Queries;

public record GetTasksQuery(string UserId, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<TaskList>>;
public record GetTaskByIdQuery(int Id, string UserId) : IRequest<TaskList?>;

public record GetTasksQuerys(string UserId, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<TaskList>>;

