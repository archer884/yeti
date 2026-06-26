using Microsoft.EntityFrameworkCore;

using Yeti.Core.Model;
using Yeti.Db;

namespace Yeti.Core.Provider;

public class FragmentDetailProvider(WriterContext context)
{
    public async Task<FragmentDetail?> ById(long id)
    {
        return await context.Fragments
            .Where(x => !x.SoftDelete)
            .Where(x => x.Id == id)
            .Select(x => new FragmentDetail(x.Id, x.WriterId, x.ManuscriptId, x.Heading, x.Content))
            .FirstOrDefaultAsync();
    }
}
