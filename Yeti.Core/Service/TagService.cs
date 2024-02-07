using Microsoft.EntityFrameworkCore;

using Yeti.Core.Model;
using Yeti.Db;
using Yeti.Db.Model;

namespace Yeti.Core.Service;

public class TagService(WriterContext context)
{
    public async Task AddTag(ModifyTag modify)
    {
        var tag = await ByValue(modify.Normalized);
        var manuscript = await context.Manuscripts
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == modify.ManuscriptId && x.WriterId == modify.WriterId);

        if (manuscript is null)
        {
            return;
        }

        manuscript.Tags.Add(tag);
        context.Update(manuscript);
        await context.SaveChangesAsync();
    }

    public async Task RemoveTag(ModifyTag modify)
    {
        var manuscript = await context.Manuscripts
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == modify.ManuscriptId && x.WriterId == modify.WriterId);

        if (manuscript is null)
        {
            return;
        }

        var tag = manuscript.Tags.FirstOrDefault(x => x.Value == modify.Normalized);
        if (tag is null)
        {
            return;
        }

        manuscript.Tags.Remove(tag);
        context.Update(manuscript);
        await context.SaveChangesAsync();
    }

    async Task<Tag> ByValue(string value)
    {
        var tag = await context.Tags.FirstOrDefaultAsync(x => x.Value == value);
        if (tag is not null)
        {
            return tag;
        }

        var added = await context.Tags.AddAsync(new Tag { Value = value });
        return added.Entity;
    }
}
