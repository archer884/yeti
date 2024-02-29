using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Yeti.Core.Model;
using Yeti.Db;

namespace Yeti.Core.Service;

public class SearchService(ILogger<SearchService> logger, IndexClient client, WriterContext context)
{
    public async Task<List<Snapshot>> Query(string query, int? page = null)
    {
        var info = await client.Query(query, page);
        if (info is null)
        {
            logger.LogError("search api provided null response to query: {query}", query);
            return [];
        }

        var ids = info.Select(x => x.Id).ToList();
        var snapshots = await context.Fragments
            .Where(x => ids.Contains(x.Id))
            .Select(x => new Snapshot(
                x.Id,
                x.WriterId,
                x.ManuscriptId,
                x.Writer.AuthorName,
                x.Manuscript.Title,
                x.Heading,
                x.Content))
            .ToListAsync();

        // No clue how to create an excerpt on the server side, so we'll do it here. 280 is the max
        // length for a standard tweet these days.
        return snapshots
            .Select(x => x with { Excerpt = x.Excerpt[..Math.Min(280, x.Excerpt.Length)] })
            .ToList();
    }
}
