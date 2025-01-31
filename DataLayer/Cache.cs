using DataLayer.DAL;
using DataLayer.Models;
using System.Runtime.Caching;

namespace DataLayer
{
	public class Cache
	{
		private static readonly Lazy<Cache> _instance = new Lazy<Cache>(() => new Cache());
		private readonly ObjectCache _cache;

		public enum CacheName
		{
			Books, DaysOfBorrow, BorrowLimitPerUser, Users
		}

		private Cache()
		{
			_cache = MemoryCache.Default;
		}

		public static Cache Instance => _instance.Value;

		public void SetCache<T>(CacheName key, T value, TimeSpan expiration)
		{
			var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.Add(expiration) };
			_cache.Set(GetEnmName(key), value, policy);
		}

		public void SetCache<T>(CacheName key, T value)
		{
			var policy = new CacheItemPolicy();
			policy.Priority = CacheItemPriority.NotRemovable;
			_cache.Set(GetEnmName(key), value, policy);
		}

		public T? GetCache<T>(CacheName key)
		{
			return (_cache.Contains(GetEnmName(key))) ? (T)_cache.Get(GetEnmName(key)) : default;
		}

		public void Remove(CacheName key)
		{
			if (_cache.Contains(GetEnmName(key)))
			{
				_cache.Remove(GetEnmName(key));
			}
		}

		private string GetEnmName(CacheName name)
		{
			var t = Enum.GetName(typeof(CacheName), name);
			return string.IsNullOrEmpty(t) ? "" : t.ToString();
		}
	}
}
