namespace OpenTableDataViz.Services
{
	using System;

	public class ApiException : Exception
	{
		public ApiException(string updateentityErrorUpdatingEntity, ErrorResponse errorData)
		{
			throw new NotImplementedException();
		}
	}
}