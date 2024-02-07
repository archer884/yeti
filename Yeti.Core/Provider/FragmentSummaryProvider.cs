using Microsoft.EntityFrameworkCore;
using Yeti.Core.Model;
using Yeti.Db;

namespace Yeti.Core.Provider;

public class FragmentSummaryProvider(WriterContext context)
{
    public Task<List<FragmentSummary>> ByManuscriptId(long id)
    {
        return context.Fragments.Where(x => x.ManuscriptId == id).Select(x => new FragmentSummary
        {
            Id = x.Id,
            WriterId = x.WriterId,
            ManuscriptId = x.ManuscriptId,
            Heading = x.Heading,
            Length = x.Content.Length,
            SortBy = x.SortBy,
        }).ToListAsync();
    }
}
