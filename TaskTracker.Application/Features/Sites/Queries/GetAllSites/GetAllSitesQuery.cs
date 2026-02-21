using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Sites.Queries.GetAllSites;

public record GetAllSitesQuery : IRequest<List<SiteDto>>;
