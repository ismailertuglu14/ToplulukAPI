using Topluluk.Shared.Dtos;

namespace DBHelper.Repository;

public interface IRedisRepository
{    
    bool IsConnected { get; }
    Task<string> GetValueAsync(string key);
    Task<Dictionary<string, string>> GetAllAsync(List<string> keys);

    /// <summary>
    /// Serializes the 'value' parameter of type T that is received from an external source.
    /// </summary>
    /// <param name="key">The key associated with the value.</param>
    /// <param name="value">The value to be serialized.</param>
    /// <typeparam name="T">The type of the value to be serialized.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating whether the serialization was successful.</returns>
    Task<bool> SetValueAsync<T>(string key, T value);
    Task<T> GetOrNullAsync<T>(string key) where T : class;
    
    
    
    
    Task<List<string>> GetListValueAsync(List<string> keys);
    Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> action) where T : class;
    T GetOrAdd<T>(string key, Func<T> action) where T : class;

    Task Clear(string key);
    void ClearAll();

}