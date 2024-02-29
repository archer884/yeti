using Microsoft.Extensions.Logging;

using Yeti.Db.Model;

namespace Yeti.Core.Service;

public class IndexingService(ILogger<IndexingService> logger, IndexClient client) : IIndexingService
{
    public void Add(Fragment fragment)
    {
        var _ = Task.Run(async () =>
        {
            try
            {
                await client.Add(fragment.Id);
            }
            catch (IndexException e)
            {
                logger.LogError(e, "failed to add fragment to index: {id}", fragment.Id);
            }
        });
    }

    public void Update(Fragment add, Fragment remove)
    {
        var _ = Task.Run(async () =>
        {
            try
            {
                await client.Update(add.Id, remove.Id);
            }
            catch (IndexException e)
            {
                logger.LogError(e, "failed to update index for fragments: {add}, {remove}", add.Id, remove.Id);
            }
        });
    }

    public void Delete(Fragment fragment)
    {
        var _ = Task.Run(async () =>
        {
            try
            {
                await client.Delete(fragment.Id);
            }
            catch (IndexException e)
            {
                logger.LogError(e, "failed to delete fragment from index: {id}", fragment.Id);
            }
        });
    }
}
