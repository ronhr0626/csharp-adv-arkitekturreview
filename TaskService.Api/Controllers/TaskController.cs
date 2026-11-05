using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskService.Api.DTO;

namespace TaskService.Api.Controllers;

[ApiController]
[Route("tasks")]
[Authorize] // Only requests with a valid token get in. Everything else gets 401.
public class TaskController : ControllerBase
{
    private readonly Core.TaskService taskManager;

    public TaskController(Core.TaskService taskService)
    {
        taskManager = taskService;
    }

    // The token is already checked, so we can read the user id ("sub") directly
    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst("sub")!.Value);
    }

    [HttpGet]
    public IActionResult GetTasks()
    {
        return Ok(taskManager.GetTasks(GetUserId()));
    }

    [HttpPost]
    public IActionResult AddTask([FromBody] CreateTaskRequest request)
    {
        return Ok(taskManager.AddTask(request.Title, GetUserId()));
    }

    [HttpPatch("{id}/complete")]
    public IActionResult CompleteTask(Guid id)
    {
        // The domain decides if the task exists for this user. The controller only maps it to HTTP.
        try
        {
            taskManager.CompleteTask(id, GetUserId());
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTask(Guid id)
    {
        try
        {
            taskManager.DeleteTask(id, GetUserId());
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}