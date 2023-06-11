using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DBHelper.Repository.Redis;

public class RedisCacheRepository : IRedisRepository
{
    private readonly IConnectionMultiplexer _redisCon;
    private readonly IDatabase _cache;
    private TimeSpan ExpireTime => TimeSpan.FromMinutes(5);
    public RedisCacheRepository(IConnectionMultiplexer redisCon)
    {
        _redisCon = redisCon;
        _cache = redisCon.GetDatabase();
        
    }


    public bool IsConnected => _redisCon.IsConnected;

    public async Task<T> GetOrNullAsync<T>(string key) where T : class
    {
        string result = await _cache.StringGetAsync(key);
        if (!result.IsNullOrEmpty())
        {
            return JsonSerializer.Deserialize<T>(result);
        }
        return null;
    }

    public async Task Clear(string key)
    {
        await _cache.KeyDeleteAsync(key);
    }
 
    public void ClearAll()
    {
        var endpoints = _redisCon.GetEndPoints(true);
        foreach (var endpoint in endpoints)
        {
            var server = _redisCon.GetServer(endpoint);
            server.FlushAllDatabases();
        }
    }

    public async Task<List<string>> GetListValueAsync(List<string> keys)
    {
        var redisKeys = keys.Select(key => (RedisKey)key).ToArray();
        var redisValues = await _cache.StringGetAsync(redisKeys);
        return redisValues.Select(value => (string)value!).ToList();
    }

    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> action) where T : class
    {
        var result = await _cache.StringGetAsync(key);
        if (result.IsNull)
        {
            result = JsonSerializer.SerializeToUtf8Bytes(await action());
            await SetValueAsync(key, result);
        }
        return JsonSerializer.Deserialize<T>(result);
    }
 
    public async Task<string> GetValueAsync(string key)
    {
        return await _cache.StringGetAsync(key);
    }

    public async Task<Dictionary<string, string>> GetAllAsync(List<string> keys)
    {
        
        var redisValues = await _cache.StringGetAsync(keys.Select(x => (RedisKey)x).ToArray());
        var result = new Dictionary<string, string>();
        for (int i = 0; i < keys.Count; i++)
        {
            result[keys[i]] = redisValues[i];
        }
        return result;
    }

    /// <summary>
    /// Auto Serialize T model
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<bool> SetValueAsync<T>(string key, T value)
    {
        var json = JsonConvert.SerializeObject(value);
        return await _cache.StringSetAsync(key,json, ExpireTime);
    }
 
    public T GetOrAdd<T>(string key, Func<T> action) where T : class
    {
        var result =  _cache.StringGet(key);
        if (result.IsNull)
        {
            result = JsonSerializer.SerializeToUtf8Bytes(action());
            _cache.StringSet(key, result,ExpireTime);
        }
        return JsonSerializer.Deserialize<T>(result);
    }

}