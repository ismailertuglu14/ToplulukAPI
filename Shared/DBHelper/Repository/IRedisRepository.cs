using Topluluk.Shared.Dtos;

namespace DBHelper.Repository;

public interface IRedisRepository
{    
    bool IsConnected { get; }
    Task<string> GetValueAsync(string key);
    Task<Dictionary<string, string>> GetAllAsync(List<string> keys);
    Task<bool> SetValueAsync(string key, string value);
    Task<List<string>> GetListValueAsync(List<string> keys);
    Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> action) where T : class;
    T GetOrAdd<T>(string key, Func<T> action) where T : class;
    Task<T> GetOrNullAsync<T>(string key) where T : class;

    Task Clear(string key);
    void ClearAll();

}