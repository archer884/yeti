using Yeti.Core.Model;
using Yeti.Db;
using Yeti.Db.Model;

namespace Yeti.Core.Service;

public class WriterService(WriterContext context)
{
    public async Task<WriterInfo?> Create()
    {
        throw new NotImplementedException();
    }

    public async Task<WriterInfo?> GetInfo(long id)
    {
        if (await GetById(id) is not Writer writer)
        {
            return null;
        }

        return WriterInfo.FromWriter(writer);
    }

    public async Task<WriterInfo?> Update(long id, UpdateWriter update)
    {
        if (await GetById(id) is not Writer writer)
        {
            return null;
        }

        writer.AuthorName = update.AuthorName;
        await context.SaveChangesAsync();
        return WriterInfo.FromWriter(writer);
    }

    private ValueTask<Writer?> GetById(long id) => context.Writers.FindAsync(id);
}
