using Yeti.Db.Model;

namespace Yeti.Core.Model;

public class ManuscriptSummary(Manuscript Manuscript)
{
    public long Id => Manuscript.Id;
    public long WriterId => Manuscript.WriterId;
    public string Title => Manuscript.Title;
    public string? Blurb => Manuscript.Blurb;
}
