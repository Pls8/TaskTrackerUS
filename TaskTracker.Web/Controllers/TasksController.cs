using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskTracker.Application.Features.Tasks.Commands.CreateTask;
using TaskTracker.Application.Features.Tasks.Commands.DeleteTask;
using TaskTracker.Application.Features.Tasks.Commands.UpdateTask;
using TaskTracker.Application.Features.Tasks.Queries.GetAllTasks;
using TaskTracker.Application.Features.Tasks.Queries.GetTaskById;
using TaskTracker.Web.ViewModels;

namespace TaskTracker.Web.Controllers;

public class TasksController : Controller
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var tasks = await _mediator.Send(new GetAllTasksQuery());
        return View(tasks);
    }

    public async Task<IActionResult> Create()
    {
        var tasks = await _mediator.Send(new GetAllTasksQuery());
        // Filter tasks that can be parents (Sections)
        var sections = tasks.Where(t => t.IsSection).OrderBy(t => t.Title).ToList();
        
        ViewBag.ParentTasks = new SelectList(sections, "Id", "Title");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskViewModel model)
    {
        if (ModelState.IsValid)
        {
            var command = new CreateTaskCommand
            {
                Title = model.Title,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Owner = model.Owner,
                Resource = model.Resource,
                Remark = model.Remark,
                TaskWeightPercentage = model.TaskWeightPercentage,
                Priority = model.Priority,
                IsSection = false, // Always false for Tasks
                ParentTaskId = model.ParentTaskId
            };

            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
        
        var tasks = await _mediator.Send(new GetAllTasksQuery());
        var sections = tasks.Where(t => t.IsSection).OrderBy(t => t.Title).ToList();
        ViewBag.ParentTasks = new SelectList(sections, "Id", "Title", model.ParentTaskId);
        return View(model);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var task = await _mediator.Send(new GetTaskByIdQuery(id));
        if (task == null)
        {
            return NotFound();
        }

        var model = new UpdateTaskViewModel
        {
            Id = task.Id,
            Description = task.Description,
            Title = task.Title,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Owner = task.Owner,
            Resource = task.Resource,
            Remark = task.Remark,
            TaskWeightPercentage = task.TaskWeightPercentage,
            TaskCompletionPercentage = task.TaskCompletionPercentage,
            Priority = task.Priority,
            IsSection = task.IsSection,
            ParentTaskId = task.ParentTaskId
        };

        var tasks = await _mediator.Send(new GetAllTasksQuery());
        // Only sections can be parents
        var sections = tasks.Where(t => t.IsSection && t.Id != id).OrderBy(t => t.Title).ToList();
        ViewBag.ParentTasks = new SelectList(sections, "Id", "Title", task.ParentTaskId);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateTaskViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var command = new UpdateTaskCommand
            {
                Id = model.Id,
                Description = model.Description,
                Title = model.Title,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Owner = model.Owner,
                Resource = model.Resource,
                Remark = model.Remark,
                TaskWeightPercentage = model.TaskWeightPercentage,
                TaskCompletionPercentage = model.TaskCompletionPercentage,
                Priority = model.Priority,
                IsSection = model.IsSection,
                ParentTaskId = model.ParentTaskId
            };

            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        var tasks = await _mediator.Send(new GetAllTasksQuery());
        var sections = tasks.Where(t => t.IsSection && t.Id != id).OrderBy(t => t.Title).ToList();
        ViewBag.ParentTasks = new SelectList(sections, "Id", "Title", model.ParentTaskId);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTaskCommand(id));
        return RedirectToAction(nameof(Index));
    }
}
