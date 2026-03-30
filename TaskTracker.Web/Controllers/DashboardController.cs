using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Features.Dashboard.Queries;

namespace TaskTracker.Web.Controllers;

[Authorize]

public class DashboardController : Controller
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(string searchTerm)
    {
        var metrics = await _mediator.Send(new GetDashboardMetricsQuery(searchTerm));
        return View(metrics);
    }
}
