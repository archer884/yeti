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
}
