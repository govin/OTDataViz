using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTableDataViz.Services
{
	public interface ICacheService
	{
		void CacheItem(string key, object item);

		object GetCacheItem(string key);
	}
}
