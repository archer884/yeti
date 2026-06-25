using Microsoft.EntityFrameworkCore;

using Yeti.Core.Model;
using Yeti.Db;

namespace Yeti.Core.Provider;

public class ManuscriptSummaryProvider(WriterContext context)
{
    public async Task<ManuscriptSummary?> ById(long id)
    {
        var manuscript = await context.Manuscripts.FindAsync(id);
        if (manuscript is null || manuscript.SoftDelete)
        {
            return null;
        }

        return new ManuscriptSummary(manuscript);
    }

    public async Task<List<ManuscriptSummary>> ByWriterId(long writerId)
    {
        var manuscripts = await context.Manuscripts
            .Where(x => x.WriterId == writerId && !x.SoftDelete)
            .OrderByDescending(x => x.Updated)
            .ToListAsync();

        return manuscripts.Select(x => new ManuscriptSummary(x)).ToList();
    }
}
