using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace DBHelper.Repository.Redis;

public class RedisCacheRepository : IRedisRepository
{
    private readonly IConnectionMultiplexer _redisCon;
    private readonly IDatabase _cache;
    private TimeSpan ExpireTime => TimeSpan.FromMinutes(5);
    private IConfiguration _configuration;
    public RedisCacheRepository(IConnectionMultiplexer redisCon,IConfiguration configuration)
    {
        _configuration = configuration;
        try
        {
            _redisCon = redisCon;
        }
        catch (Exception e)
        {
            throw new Exception("Redis bağlantısı başarısız oldu.");
        }
        _cache = redisCon.GetDatabase();
    }
    public bool CheckRedisConnection()
    {
        try
        {
            var redis = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
            return true;
        }
        catch
        {
            return false;
        }
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
 
    public async Task<bool> SetValueAsync(string key, string value)
    {
        return await _cache.StringSetAsync(key,value, ExpireTime);
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