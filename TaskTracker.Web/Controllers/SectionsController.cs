using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Features.Tasks.Commands.CreateTask;
using TaskTracker.Application.Features.Tasks.Commands.DeleteTask;
using TaskTracker.Application.Features.Tasks.Commands.UpdateTask;
using TaskTracker.Application.Features.Tasks.Queries.GetAllTasks;
using TaskTracker.Application.Features.Tasks.Queries.GetTaskById;
using TaskTracker.Domain.Enums;
using TaskTracker.Web.ViewModels;

namespace TaskTracker.Web.Controllers;

public class SectionsController : Controller
{
    private readonly IMediator _mediator;

    public SectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var tasks = await _mediator.Send(new GetAllTasksQuery());
        // Filter only sections
        var sections = tasks.Where(t => t.IsSection).OrderBy(t => t.Title).ToList();
        return View(sections);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskViewModel model)
    {
        // Force IsSection to true
        model.IsSection = true;
        
        // Sections don't need ParentTaskId
        model.ParentTaskId = null;

        if (ModelState.IsValid)
        {
            var command = new CreateTaskCommand
            {
                Title = model.Title,
                Description = model.Description,
                StartDate = model.StartDate ?? DateTime.UtcNow,
                EndDate = model.EndDate ?? DateTime.UtcNow.AddDays(30),
                Owner = model.Owner ?? "Project Manager",
                TaskWeightPercentage = model.TaskWeightPercentage,
                Priority = model.Priority,
                IsSection = true,
                ParentTaskId = null
            };

            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var task = await _mediator.Send(new GetTaskByIdQuery(id));
        if (task == null || !task.IsSection)
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
            TaskWeightPercentage = task.TaskWeightPercentage,
            Priority = task.Priority,
            IsSection = true
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateTaskViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        model.IsSection = true;

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
                IsSection = true,
                ParentTaskId = null
            };

            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTaskCommand(id));
        return RedirectToAction(nameof(Index));
    }
}
