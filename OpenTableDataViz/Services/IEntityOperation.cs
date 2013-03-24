using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTableDataViz.Services
{
	public interface IEntityOperation
	{
		T GetEntity<T>(string url);

		TR CreateEntity<T, TR>(T entity, string url);

		TR UpdateEntity<T, TR>(T entity, string url);

		TR DeleteEntity<TR>(string url);
	}
}
