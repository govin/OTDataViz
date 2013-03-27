namespace OpenTableDataViz.Services
{
	using System.Web;

	public class CacheService : ICacheService
	{
		public void CacheItem(string key, object item)
		{
			HttpRuntime.Cache.Insert(key, item);
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