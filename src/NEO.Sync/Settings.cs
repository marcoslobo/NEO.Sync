using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEO.Api.Worker
{
    public class Settings
    {
		public static string ParitySettings = "parityRopstenSettings";

		public static string TestNetSettings = "Settings";

		public string CurrentSettings = TestNetSettings;

		public IConfigurationRoot Configuration
		{
			get;
			set;
		}

		public Settings()
		{
			IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("settings.json");
			Configuration = builder.Build();
		}

		public string GetDefaultAccount()
		{
			return GetAppSettingsValue("defaultAccount");
		}

		public string GetBlockHash()
		{
			return GetAppSettingsValue("blockhash");
		}

		public ulong GetBlockNumber()
		{
			return Convert.ToUInt64(GetAppSettingsValue("blockNumber"));
		}

		public string GetContractHash()
		{
			return GetAppSettingsValue("contractHash");
		}

		public string GetContractTransaction()
		{
			return GetAppSettingsValue("contractTransaction");
		}

		public string GetMinerTransaction()
		{
			return GetAppSettingsValue("minerTransaction");
		}

		public string GetClaimTransaction()
		{
			return GetAppSettingsValue("claimTransaction");
		}

		public string GetGoverningAssetHash()
		{
			return GetAppSettingsValue("governingAssetHash");
		}

		public string GetUtilityAssetHash()
		{
			return GetAppSettingsValue("utilityAssetHash");
		}

		public string GetOokenAssetHash()
		{
			return GetAppSettingsValue("tokenAssetHash");
		}

		private string GetAppSettingsValue(string key)
		{
			return GetSectionSettingsValue(key, CurrentSettings);
		}

		private string GetSectionSettingsValue(string key, string sectionSettingsKey)
		{
			IConfigurationSection setting = Configuration.GetSection(sectionSettingsKey).GetChildren().FirstOrDefault((IConfigurationSection x) => x.Key == key);
			if (setting != null)
			{
				return setting.Value;
			}
			throw new Exception("Setting: " + key + " Not found");
		}

		public string GetRpcUrl()
		{
			return GetAppSettingsValue("rpcUrl");
		}

		public string GetDefaultLogLocation()
		{
			return GetAppSettingsValue("debugLogLocation");
		}
	}
}
