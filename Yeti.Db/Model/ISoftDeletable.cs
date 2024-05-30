namespace Yeti.Db.Model;

public interface ISoftDeletable
{
    bool SoftDelete { get; set; }
}
