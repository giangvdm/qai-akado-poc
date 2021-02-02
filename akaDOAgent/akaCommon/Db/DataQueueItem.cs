using System;

namespace akaCommon.Db
{
	public class DataQueueItem
	{
		public DataQueueItem()
		{
			this.Id = 0;
		}
		public int Id { get; set; }
		public string JsonData { get; set; }
		public DateTime CollectdTime { get; set; }
	}
}
