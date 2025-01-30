using DataLayer.DAL;
using DataLayer.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataLayer
{
	public static class Cache
	{
		public enum CacheName
		{
			Books, DaysOfBorrow, BorrowLimitPerUser, Users
		}
		private static MemoryCache CacheObj = new MemoryCache(new MemoryCacheOptions());

#nullable disable   // Bypass unnecessary warnings.
		public static async void InitAllData(EdtBookingContext context)
		{
			DalBooks book = new DalBooks(context);
			await book.GetAllBooks();  // Initialize the data in memory to enhance the search process speed.
		}

		public static T GetCache<T>(CacheName name)
		{
			return (T)CacheObj.Get(GetEnmName(name));
		}

		public static void SetCache<T>(CacheName name, T value) where T : new()
		{
			CacheObj.Set(GetEnmName(name), value, new MemoryCacheEntryOptions());
		}

		public static void SetCache(CacheName name, string value)
		{
			CacheObj.Set(GetEnmName(name), value, new MemoryCacheEntryOptions());
		}
#nullable enable

		public static void ClearCache()
		{
			CacheObj.Clear();
		}

		private static string GetEnmName(CacheName name)
		{
			var t = Enum.GetName(typeof(CacheName), name);
			return string.IsNullOrEmpty(t) ? "" : t.ToString();
		}
	}
}
