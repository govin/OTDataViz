using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTableDataViz.Services
{
	public interface ICacheService
	{
		void SetCacheItem(string key, object item);

		void SetCacheItem(string key, object item, int timeoutMinutes);

		object GetCacheItem(string key);
	}
}
