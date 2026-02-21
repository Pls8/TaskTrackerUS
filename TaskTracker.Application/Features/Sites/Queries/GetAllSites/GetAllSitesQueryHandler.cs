using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Sites.Queries.GetAllSites;

public class GetAllSitesQueryHandler : IRequestHandler<GetAllSitesQuery, List<SiteDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllSitesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SiteDto>> Handle(GetAllSitesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ProjectSites
            .ProjectTo<SiteDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
