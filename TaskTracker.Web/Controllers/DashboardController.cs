using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Features.Dashboard.Queries;

namespace TaskTracker.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var metrics = await _mediator.Send(new GetDashboardMetricsQuery());
        return View(metrics);
    }
}
