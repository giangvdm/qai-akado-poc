using akaCommon;
using System;
using System.Management;

namespace akaSensor.Windows
{
	public class CommonInfo
	{
		private static CommonInfo smCommonInfo;

		private void Initialize()
		{
			if (HostInfo == null)
			{
				HostInfo hostInfo = new HostInfo();
				hostInfo.computer_name = "abc";
				hostInfo.domain_name = "xyz";
				hostInfo.uuid = "12312";
				hostInfo.serial_number = "12323";
				hostInfo.mac_address = "61351";
				HostInfo = hostInfo;
			}
		}

		static public CommonInfo Instance
		{
			get
			{
				if (smCommonInfo == null)
				{
					smCommonInfo = new CommonInfo();
					smCommonInfo.Initialize();
				}
				return smCommonInfo;
			}
		}

		public HostInfo HostInfo { get; private set; }
	}
}
