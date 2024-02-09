using Microsoft.EntityFrameworkCore;

using Yeti.Core.Model;
using Yeti.Db;

namespace Yeti.Core.Service;

public class RecentService(WriterContext context)
{
    const int PageSize = 10;

    public async Task<List<ManuscriptSummary>> ByNew(int page)
    {
        var manuscripts = await context.Manuscripts
            .OrderByDescending(x => x.Created)
            .Skip(page * PageSize)
            .Take(PageSize)
            .ToListAsync();
        
        return manuscripts.Select(x => new ManuscriptSummary(x)).ToList();
    }

    public async Task<List<ManuscriptSummary>> ByUpdated(int page)
    {
        var manuscripts = await context.Fragments
            .Include(x => x.Manuscript)
            .DistinctBy(x => x.ManuscriptId)
            .OrderByDescending(x => x.Created)
            .Skip(page * PageSize)
            .Take(PageSize)
            .Select(x => x.Manuscript)
            .ToListAsync();
        
        return manuscripts.Select(x => new ManuscriptSummary(x)).ToList();
    }
}
