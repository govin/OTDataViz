using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Services
{
	using System.Net.Http;
	using System.Threading.Tasks;


		using System;
	

	public class EntityOpService : IEntityOperation
		{
			private readonly ILogger logger;

			private readonly IAppConfiguration appConfiguration;

			public EntityOpService(ILogger logger, IAppConfiguration appConfiguration)
			{
				this.logger = logger;
				this.appConfiguration = appConfiguration;
			}

			public T GetEntity<T>(string url)
			{
				T retData = default(T);
				ErrorResponse errorData = null;
				try
				{
					var client = new HttpClient();

					var task = client.GetAsync(url).ContinueWith(
						request =>
						{
							
							var response = request.Result;

							if (response.IsSuccessStatusCode)
							{
								// read the response as strongly typed object
								var successTask = response.Content.ReadAsAsync<T>().ContinueWith(
									readTask =>
									{
										retData = readTask.Result;
									});
								successTask.Wait();
							}
							else
							{
								// error response
								var errorTask = response.Content.ReadAsAsync<ErrorResponse>().ContinueWith(readTask => { errorData = readTask.Result; });
								errorTask.Wait();
								throw new ApiException("GetEntity(): Error getting entity", errorData);
							}
						});

					task.Wait();
				}
				catch (Exception ex)
				{
					this.logger.LogError(ex);
					throw;
				}

				return retData;
			}

			public TR CreateEntity<T, TR>(T entity, string url)
			{
				var retData = default(TR);
				ErrorResponse errorData = null;
				try
				{
					var client = new HttpClient();

					var task = client.PostAsJsonAsync(url, entity).ContinueWith(
						request =>
						{
							var response = request.Result;

							if (response.IsSuccessStatusCode)
							{
								// read the response as strongly typed object
								var successTask = response.Content.ReadAsAsync<TR>().ContinueWith(
									readTask =>
									{
										retData = readTask.Result;
									});
								successTask.Wait();
							}
							else
							{
								// error response
								var errorTask = response.Content.ReadAsAsync<ErrorResponse>().ContinueWith(readTask => { errorData = readTask.Result; });
								errorTask.Wait();
								throw new ApiException("CreateEntity(): Error creating entity", errorData);
							}
						});

					task.Wait();
				}
				catch (Exception ex)
				{
					this.logger.LogError(ex);
					throw;
				}

				return retData;
			}

			public TR UpdateEntity<T, TR>(T entity, string url)
			{
				var retData = default(TR);
				ErrorResponse errorData = null;
				try
				{
					var client = new HttpClient();

					var task = client.PutAsJsonAsync(url, entity).ContinueWith(
						request =>
						{
							var response = request.Result;

							if (response.IsSuccessStatusCode)
							{
								// read the response as strongly typed object
								var successTask = response.Content.ReadAsAsync<TR>().ContinueWith(
									readTask =>
									{
										retData = readTask.Result;
									});
								successTask.Wait();
							}
							else
							{
								// error response
								var errorTask = response.Content.ReadAsAsync<ErrorResponse>().ContinueWith(readTask => { errorData = readTask.Result; });
								errorTask.Wait();
								throw new ApiException("UpdateEntity(): Error updating entity", errorData);
							}
						});

					task.Wait();
				}
				catch (Exception ex)
				{
					this.logger.LogError(ex);
					throw;
				}

				return retData;
			}

			public TR DeleteEntity<TR>(string url)
			{
				var retData = default(TR);
				ErrorResponse errorData = null;
				try
				{
					var client = new HttpClient();

					var task = client.DeleteAsync(url).ContinueWith(request =>
					{
						var response = request.Result;

						if (!response.IsSuccessStatusCode)
						{
							// error response
							var errorTask = response.Content.ReadAsAsync<ErrorResponse>().ContinueWith(readTask => { errorData = readTask.Result; });
							errorTask.Wait();
							throw new ApiException("DeleteEntity(): Error deleting entity", errorData);
						}
					});

					task.Wait();
				}
				catch (Exception ex)
				{
					logger.LogError(ex);
					throw;
				}

				return retData;
			}

			private void HandleError(Task task)
			{
				if (task.Exception != null)
				{
					logger.LogError(task.Exception);
				}
			}
		}
}