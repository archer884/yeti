using Microsoft.EntityFrameworkCore;

using Yeti.Core.Model;
using Yeti.Core.Provider;
using Yeti.Db;
using Yeti.Db.Model;

namespace Yeti.Core.Service;

public class FragmentService(WriterContext context, FragmentSummaryProvider fragmentSummaryProvider)
{
    public async Task<FragmentSummary?> CreateFragment(long writerId, CreateFragment create)
    {
        var fragments = await fragmentSummaryProvider.ByManuscriptId(create.ManuscriptId);
        var nextSortBy = fragments.Select(x => x.SortBy).Max() + 1.0;
        var fragment = await context.Fragments.AddAsync(new Fragment
        {
            WriterId = writerId,
            ManuscriptId = create.ManuscriptId,
            Heading = create.Heading,
            Content = create.Content,
            SortBy = nextSortBy,
        });

        await context.SaveChangesAsync();
        return new FragmentSummary(fragment.Entity);
    }

    // FIXME: Probably better to split fragment updates into different members for efficiency.
    public async Task<FragmentSummary?> UpdateFragment(long writerId, UpdateFragment update)
    {
        var fragment = await context.Fragments
            .FirstOrDefaultAsync(x => x.Id == update.FragmentId && x.WriterId == writerId);

        if (fragment is null)
        {
            return null;
        }

        fragment.SoftDelete = true;
        Fragment updated = new()
        {
            WriterId = fragment.WriterId,
            ManuscriptId = fragment.ManuscriptId,
            Heading = update.Heading,
            Content = update.Content,
            SortBy = update.SortBy ?? fragment.SortBy,
        };

        context.Update(fragment);
        var result = await context.AddAsync(updated);
        await context.SaveChangesAsync();
        return new FragmentSummary(result.Entity);
    }

    public async Task<FragmentSummary?> DeleteFragment(long writerId, DeleteFragment delete)
    {
        var fragment = await context.Fragments
            .FirstOrDefaultAsync(x => x.Id == delete.FragmentId && x.WriterId == writerId);

        if (fragment is null)
        {
            return null;
        }

        fragment.SoftDelete = true;
        context.Update(fragment);
        await context.SaveChangesAsync();
        return new FragmentSummary(fragment);
    }

    public async Task<FragmentDetail?> GetFragmentDetail(long id)
    {
        var fragment = await context.Fragments
            .Where(x => !x.SoftDelete)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (fragment is null)
        {
            return null;
        }

        return new FragmentDetail(fragment.Id, fragment.WriterId, fragment.ManuscriptId, fragment.Heading, fragment.Content);
    }
}
