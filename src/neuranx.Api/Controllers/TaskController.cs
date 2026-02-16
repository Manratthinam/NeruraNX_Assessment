using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using neuranx.Application.Common.Models;
using neuranx.Application.Tasks.Commands;
using neuranx.Application.Tasks.Queries;
using neuranx.Domain.Entities;

namespace neuranx.Api.Controllers;

[Authorize]
public class TaskController : BaseController<TaskController>
{

    [HttpGet("GetTasks")]
    public async Task<ActionResult<PaginatedResult<TaskItem>>> GetTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string user = "All")
    {
        var userId = CurrentUserService.UserId;
        if (user == "All")
        {
            return Ok(await Mediator.Send(new GetTasksQuerys(userId, pageNumber, pageSize)));
        }
        else
        {
            return Ok(await Mediator.Send(new GetTasksQuery(userId, pageNumber, pageSize)));
        }

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskList>> GetTask(int id)
    {
        var userId = CurrentUserService.UserId;
        var task = await Mediator.Send(new GetTaskByIdQuery(id, userId));
        if (task == null) return NotFound();
        return task;
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask(TaskDto dto)
    {

        var userId = CurrentUserService.UserId;
        var task = await Mediator.Send(new CreateTaskCommand(dto.Title, dto.Description, dto.Status ?? "Pending", dto.AssignedToUserId, userId));
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, TaskDto dto)
    {
        var userId = CurrentUserService.UserId;
        try
        {
            await Mediator.Send(new UpdateTaskCommand(id, dto.Title, dto.Description, dto.Status ?? "Pending", userId));
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = CurrentUserService.UserId;
        try
        {
            await Mediator.Send(new DeleteTaskCommand(id, userId));
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}

public class TaskDto
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Status { get; set; } = "Pending";
    public string? AssignedToUserId { get; set; }
}
