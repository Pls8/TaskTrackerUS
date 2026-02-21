using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Sites.Queries.GetSiteStatus;

public class GetSiteStatusQueryHandler : IRequestHandler<GetSiteStatusQuery, SiteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSiteStatusQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SiteDto> Handle(GetSiteStatusQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectSites
            .Include(s => s.Tasks)
            .FirstOrDefaultAsync(s => s.Id == request.SiteId, cancellationToken);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Site with ID {request.SiteId} not found.");
        }

        return _mapper.Map<SiteDto>(entity);
    }
}
