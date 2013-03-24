using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTableDataViz.Services
{
	using System.Diagnostics;
	using System.IO;

	using MongoDB.Bson;
	using MongoDB.Driver;

	public interface IMongoDBService
	{
		MongoCollection<T> GetCollection<T>(string collectionName);
	}

	public class MongoDBService : IMongoDBService
	{
		private IAppConfiguration appConfiguration;

		private ILogger logger;

		private MongoServer _server = null;

		private MongoDatabase _database = null;

		public MongoDBService(IAppConfiguration appConfiguration, ILogger logger)
		{
			this.appConfiguration = appConfiguration;
			this.logger = logger;
		}

		public bool TestConnection()
		{
			return TestConnection(appConfiguration.ConnectionStringDataVizDB);
		}

		public static bool TestConnection(string connectionString)
		{
			try
			{
				var test = new MongoClient(connectionString).GetServer();
				if (null != test)
				{
					test.Connect(new TimeSpan(0, 0, 10));

					return true;
				}
			}
			catch (Exception)
			{
				//return false;
			}

			return false;
		}

		public string DatabaseName
		{
			get
			{
				return appConfiguration.DatabaseNameDataVizDB;
			}
		}

		public void Init()
		{
			this.GetDatabase();
		}

		protected MongoServer GetServer()
		{
			if (null == this._server)
			{
				this._server = new MongoClient(appConfiguration.ConnectionStringDataVizDB).GetServer();
			}

			return this._server;
		}

		public MongoDatabase GetDatabase()
		{
			if (null == this._database)
			{
				this._database = this.GetServer().GetDatabase(appConfiguration.DatabaseNameDataVizDB);
			}

			return this._database;
		}

		public string GetMongoPath()
		{
			// DEFERRED: support other mongodb paths (this is only used for automated testing)
			return "C:\\mongodb\\bin";
		}

		public MongoCollection<T> GetCollection<T>(string collectionName)
		{
			return this.GetDatabase().GetCollection<T>(collectionName);
		}

		public MongoCollection<T> GetCollection<T>()
		{
			return this.GetDatabase().GetCollection<T>(this.GetCollectionName<T>());
		}

		public string GetCollectionName<TEntity>()
		{
			// NOTE: The collection name will be bound to the data model class name. So we can't rename classes! But this is true of data members, too!
			return typeof(TEntity).Name;
		}
	}
}
