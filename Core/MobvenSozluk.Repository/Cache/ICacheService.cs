namespace MobvenSozluk.Repository.Cache;

public interface ICacheService<T>
{
    T Get<T>(string key);
    bool Set(string key, T value, DateTimeOffset expirationTime);
    object Remove(string key);
    bool Exists(string key);
}