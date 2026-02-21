using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Sites.Queries.GetSiteStatus;

public record GetSiteStatusQuery(Guid SiteId) : IRequest<SiteDto>;
