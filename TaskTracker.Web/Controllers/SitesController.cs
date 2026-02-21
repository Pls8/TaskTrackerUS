using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Features.Sites.Queries.GetAllSites;

namespace TaskTracker.Web.Controllers;

public class SitesController : Controller
{
    private readonly IMediator _mediator;

    public SitesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var sites = await _mediator.Send(new GetAllSitesQuery());
        return View(sites);
    }
}
