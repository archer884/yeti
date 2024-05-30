using Microsoft.EntityFrameworkCore;

using Yeti.Core.Model;
using Yeti.Core.Provider;
using Yeti.Db;
using Yeti.Db.Model;

namespace Yeti.Core.Service;

public class ManuscriptService(
    WriterContext context,
    ManuscriptSummaryProvider manuscriptSummaryProvider,
    FragmentSummaryProvider fragmentSummaryProvider)
{
    public async Task<ManuscriptSummary?> CreateManuscript(long writerId, CreateManuscript create)
    {
        var manuscript = await context.Manuscripts.AddAsync(new Manuscript
        {
            WriterId = writerId,
            Title = create.Title,
            Blurb = create.Blurb,
        });
        await context.SaveChangesAsync();
        return new ManuscriptSummary(manuscript.Entity);
    }

    public async Task<ManuscriptSummary?> UpdateManuscript(long writerId, UpdateManuscript update)
    {
        var manuscript = await context.Manuscripts
            .FirstOrDefaultAsync(x => x.Id == update.ManuscriptId && x.WriterId == writerId);

        if (manuscript is null)
        {
            return null;
        }

        manuscript.Title = update.Title;
        manuscript.Blurb = update.Blurb;
        context.Update(manuscript);
        await context.SaveChangesAsync();
        return new ManuscriptSummary(manuscript);
    }

    public async Task<ManuscriptSummary?> DeleteManuscript(long writerId, DeleteManuscript delete)
    {
        var manuscript = await context.Manuscripts
            .FirstOrDefaultAsync(x => x.Id == delete.ManuscriptId && x.WriterId == writerId);

        if (manuscript is null)
        {
            return null;
        }

        context.Remove(manuscript);
        await context.SaveChangesAsync();
        return new ManuscriptSummary(manuscript);
    }

    public Task<ManuscriptSummary?> ManuscriptSummary(long id)
    {
        return manuscriptSummaryProvider.ById(id);
    }

    public Task<List<FragmentSummary>> FragmentSummaries(long id)
    {
        return fragmentSummaryProvider.ByManuscriptId(id);
    }
}