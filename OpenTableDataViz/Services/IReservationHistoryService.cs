using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTableDataViz.Services
{
	using OpenTableDataViz.Models;

	public interface IReservationHistoryService
	{
		List<ReservationHistoryModel> GetHistory(string isoStartDate, string isoEndDate);
	}
}
