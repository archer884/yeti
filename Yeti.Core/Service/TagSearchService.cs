using Microsoft.EntityFrameworkCore;

using Yeti.Core.Model;
using Yeti.Db;

namespace Yeti.Core.Service;

public class TagSearchService(WriterContext context)
{
    const int PageSize = 25;

    public async Task<List<ManuscriptSummary>> ByTag(string value, int page)
    {
        var normalized = value.ToLowerInvariant();
        var tag = await context.Tags
            .Include(x => x.Manuscripts.OrderBy(x => x.Title).Skip(page * PageSize).Take(PageSize))
            .FirstOrDefaultAsync(x => x.Value == normalized);

        if (tag is null)
        {
            return [];
        }

        return tag.Manuscripts.Select(x => new ManuscriptSummary(x)).ToList();
    }

    public async Task<List<ManuscriptSummary>> ByTags(List<string> values, int page)
    {
        var normalized = values.Select(x => x.ToLowerInvariant()).ToList();
        var manuscripts = await context.Manuscripts
            .Where(x => normalized.All(y => x.Tags.Select(z => z.Value).Contains(y)))
            .OrderBy(x => x.Title).Skip(page * PageSize).Take(PageSize)
            .ToListAsync();

        return manuscripts.Select(x => new ManuscriptSummary(x)).ToList();
    }
}
