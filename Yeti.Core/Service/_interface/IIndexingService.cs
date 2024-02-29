using Yeti.Db.Model;

namespace Yeti.Core.Service;

public interface IIndexingService
{
    public void Add(Fragment fragment);
    public void Update(Fragment add, Fragment remove);
    public void Delete(Fragment fragment);
}
