using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace akaCommon.Db
{
	public class DataQueue
	{
		readonly List<DataQueueItem> _dataQueues = new List<DataQueueItem>();
		readonly List<int> _lstRemoved = new List<int>();
		private static DataQueue _sDataQueue;
		private static object _oLocker = new object();

		private DataQueue()
		{
			InitializeDatabase();
		}

		public static DataQueue Instance
		{
			get
			{
				lock (_oLocker)
				{
					if (_sDataQueue == null)
					{
						_sDataQueue = new DataQueue();
						_sDataQueue.ReadDb();
					}
				}
				return _sDataQueue;
			}
		}

		public void Add(DataQueueItem queueItem)
		{
			lock (_oLocker)
			{
				if (_dataQueues.Any(x => x.CollectdTime >= queueItem.CollectdTime))
				{
					return;
				}
				_dataQueues.Add(queueItem);
			}
		}

		public void Dequeue()
		{
			lock (_oLocker)
			{
				if(_dataQueues.Count > 0)
				{
					DataQueueItem queueItem = _dataQueues[0];
					_dataQueues.RemoveAt(0);
					if (queueItem.Id > 0)
					{
						_lstRemoved.Add(queueItem.Id);
					}
				}
			}
		}

		public DataQueueItem Peek()
		{
			lock (_oLocker)
			{
				return _dataQueues.Count > 0 ? _dataQueues[0] : null;
			}
		}

		public void WriteToDb()
		{
			lock (_oLocker)
			{
				if ((_dataQueues.Count == 0 || _dataQueues.All(x => x.Id > 0)) && _lstRemoved.Count == 0)
				{
					return;
				}

				using (SqliteConnection db = new SqliteConnection($"Filename={AppInfo.DatabaseFile}"))
				{
					db.Open();
					SqliteTransaction transaction = null;
					try
					{
						transaction = db.BeginTransaction();

						foreach(int nId in _lstRemoved)
						{
							string strQuery = $"DELETE FROM PendingData WHERE Id=@Id ";

							using (SqliteCommand command = db.CreateCommand())
							{
								command.Transaction = transaction;
								command.CommandText = strQuery;
								command.Parameters.AddWithValue("@Id", nId);
								command.ExecuteNonQuery();
							}
						}

						foreach (DataQueueItem dataQueueItem in _dataQueues.Where(x => x.Id == 0))
						{
							string strQuery = $"INSERT INTO PendingData(JsonData, CollectdTime) VALUES(@JsonData, @CollectdTime) ";
							using (SqliteCommand command = db.CreateCommand())
							{
								command.Transaction = transaction;
								command.CommandText = strQuery;
								command.Parameters.AddWithValue("@JsonData", dataQueueItem.JsonData);
								command.Parameters.AddWithValue("@CollectdTime", dataQueueItem.CollectdTime.Ticks);
								object ret = command.ExecuteScalar();

								if (ret != null)
								{
									int nId = int.Parse(ret.ToString());
									dataQueueItem.Id = nId;
								}
							}
						}
						transaction.Commit();
						_lstRemoved.Clear();
						_dataQueues.Clear();
					}
					catch(Exception)
					{
						if (transaction != null)
						{
							transaction.Rollback();
						}
						throw;
					}
					finally
					{
						if (transaction != null)
						{
							transaction.Dispose();
						}
					}
				}
			}
		}

		public void ReadDb()
		{
			lock (_oLocker)
			{
				List<DataQueueItem> lstDbItems = new List<DataQueueItem>();
				using (SqliteConnection db = new SqliteConnection($"Filename={AppInfo.DatabaseFile}"))
				{
					db.Open();
					string strQuery = $"SELECT * FROM PendingData";
					using (SqliteCommand command = db.CreateCommand())
					{
						command.CommandText = strQuery;
						using (SqliteDataReader dataReader = command.ExecuteReader())
						{
							while (dataReader.Read())
							{
								DataQueueItem item = new DataQueueItem()
								{
									Id = int.Parse(dataReader["Id"].ToString()),
									JsonData = dataReader["JsonData"].ToString(),
									CollectdTime = new DateTime(long.Parse(dataReader["CollectdTime"].ToString())),
								};
								lstDbItems.Add(item);
							}
						}
					}
				}

				var validItems = _dataQueues.Where(x => !lstDbItems.Any(y => x.Id == y.Id));

				lstDbItems.InsertRange(0, validItems);
				lstDbItems = lstDbItems.OrderBy(x => x.CollectdTime).ToList();
				_dataQueues.Clear();
				_dataQueues.AddRange(lstDbItems);
			}
		}

		private void InitializeDatabase()
		{
			using (SqliteConnection db = new SqliteConnection($"Filename={AppInfo.DatabaseFile}"))
			{
				const string strCreate = "CREATE TABLE IF NOT EXISTS PendingData(Id INTEGER NOT NULL, JsonData TEXT NOT NULL, CollectdTime INTEGER NOT NULL, PRIMARY KEY(Id AUTOINCREMENT))";
				db.Open();
				using (SqliteCommand command = db.CreateCommand())
				{
					command.CommandText = strCreate;
					command.ExecuteNonQuery();
				}
			}
		}
	}
}
