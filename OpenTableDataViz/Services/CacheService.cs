using System;

namespace OpenTableDataViz.Services
{
    using System.Web;

    public class CacheService : ICacheService
    {
        public void SetCacheItem(string key, object item)
        {
            HttpRuntime.Cache.Insert(key, item);
        }

		public void SetCacheItem(string key, object item, int timeoutMinutes)
		{
			HttpRuntime.Cache.Insert(key, item, null, DateTime.Now.AddMinutes(timeoutMinutes),
				System.Web.Caching.Cache.NoSlidingExpiration);
		}


        public object GetCacheItem(string key)
        {
            return HttpRuntime.Cache[key];
        }
    }

    public static class CacheKey
    {
        public static string Restaurant = "Restaurant";
    }
}