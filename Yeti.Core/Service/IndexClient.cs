namespace Yeti.Core.Service;

public class IndexClient(HttpClient client)
{
    public async Task Add(long id)
    {
        var response = await client.PostAsync($"?a={id}", null);
        if (!response.IsSuccessStatusCode)
        {
            throw new IndexException(id);
        }
    }

    public async Task Delete(long id)
    {
        var response = await client.PostAsync($"?r={id}", null);
        if (!response.IsSuccessStatusCode)
        {
            throw new IndexException(id);
        }
    }

    public async Task Update(long add, long remove)
    {
        var response = await client.PostAsync($"?a={add}&r={remove}", null);
        if (!response.IsSuccessStatusCode)
        {
            throw new IndexException(add);
        }
    }
}
