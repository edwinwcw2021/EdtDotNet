using NLog.Config;
using NLog.Targets;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DataLayer.Models;
using System.Text.Json;

namespace DataLayer
{
	public class Common
	{
		private readonly Logger logger;
		private IConfiguration _configuration;
		private static readonly Lazy<Common> _instance = new Lazy<Common>(() => new Common());
		public static Common Instance => _instance.Value;
		private List<StatusCode> statusCodes = new List<StatusCode>();
		private Common() 
		{
			logger = NLog.LogManager.GetCurrentClassLogger();
			this.initStatusCode();
		}

		public void Initialize(IConfiguration configuration)
		{
			this._configuration = configuration;
		}

		#nullable enable
		private void initStatusCode()
		{
			string filePath = "statuscode.json";
			string jsonString = File.ReadAllText(filePath);
			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; 
			this.statusCodes = JsonSerializer.Deserialize<List<StatusCode>>(jsonString, options);
		}

		public StatusCode? GetStatusInfoFromCode(int code)
		{
			return this.statusCodes.Where(x => x.code == code).FirstOrDefault();
		}


		public void CreateLogger(string? logPath)
		{
			var config = new LoggingConfiguration();
			var fileTarget = new FileTarget
			{
				FileName = logPath + "/${shortdate}.log",
				Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} [${uppercase:${level}}] ${message}",
			};
			var fileTargetErr = new FileTarget
			{
				FileName = logPath + "/err${shortdate}.log",
				Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss} [${uppercase:${level}}] ${message}",
			};
			config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Warn, fileTarget);
			config.AddRule(NLog.LogLevel.Error, NLog.LogLevel.Fatal, fileTargetErr);
			LogManager.Configuration = config;
		}
		public void AppLog(string strMessage)
		{
			logger.Info(strMessage);
		}

		public void ErrLog(string strMessage)
		{
			logger.Error(strMessage);
		}

		public string GetAppSetting(string name)
		{
			string? obj = this._configuration[name];
			return (obj == null) ? string.Empty : obj.ToString();
		}

		public string ProblemCodeToType(int code)
		{
			string ret = "";
			switch (code)
			{
				case 400:
					ret = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
					break;
				case 401:
					ret = "https://tools.ietf.org/html/rfc7235#section-3.1";
					break;
				case 403:
					ret = "https://tools.ietf.org/html/rfc9110#section-15.5.4";
					break;
				case 404:
					ret = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
					break;
				case 405:
					ret = "https://tools.ietf.org/html/rfc9110#section-15.5.6";
					break;
				case 409:
					ret = "https://tools.ietf.org/html/rfc9110#section-15.5.10";
					break;
				case 415:
					ret = "https://tools.ietf.org/html/rfc9110#section-15.5.16";
					break;
				case 429:
					ret = "https://tools.ietf.org/html/rfc6585#section-4";
					break;
				case 500:
					ret = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
					break;
				case 503:
					ret = "https://tools.ietf.org/html/rfc9110#section-15.6.4";
					break;
			};
			return ret;
		}

		public List<T> CloneRecClass<T>(List<T> fromList)
		{
			List<T> ret = new List<T>();
			foreach (var r in fromList)
			{
				T newObj = Activator.CreateInstance<T>();
				CopyRecClass<T, T>(r, newObj);
				ret.Add(newObj);
			}
			return ret;
		}

#nullable disable   // Bypass unnecessary warnings.
		public int GetNoOfBorrowDays()
		{
			var obj = Cache.Instance.GetCache<string>(Cache.CacheName.DaysOfBorrow);
			if (obj == null)
			{
				var DaysOfBorrow = GetAppSetting("DaysOfBorrow");
				Cache.Instance.SetCache(Cache.CacheName.DaysOfBorrow, DaysOfBorrow);
				return Convert.ToInt32(DaysOfBorrow);
			}
			return Convert.ToInt32(obj);
		}

		public int GetBorrowLimitPerUser()
		{
			var obj = Cache.Instance.GetCache<string?>(Cache.CacheName.BorrowLimitPerUser);
			if (obj == null)
			{
				var BorrowLimitPerUser = GetAppSetting("BorrowLimitPerUser");
				Cache.Instance.SetCache(Cache.CacheName.BorrowLimitPerUser, BorrowLimitPerUser);
				return Convert.ToInt32(BorrowLimitPerUser);
			}
			return Convert.ToInt32(obj);
		}

		public void CopyRecClass<T, TT>(T fromClass, TT toClass)
		{
			System.Reflection.PropertyInfo[] toClassProperties;
			toClassProperties = toClass.GetType().GetProperties();
			foreach (System.Reflection.PropertyInfo toClassProperty in toClassProperties)
			{
				if (toClassProperty.Name != "EntityState" && toClassProperty.Name != "EntityKey")
				{
					if (toClass.GetType().GetProperty(toClassProperty.Name).PropertyType.Namespace != toClass.GetType().Namespace)
					{
						if (fromClass.GetType().GetProperty(toClassProperty.Name) != null)
						{
							toClass.GetType().GetProperty(toClassProperty.Name).SetValue(toClass, fromClass.GetType().GetProperty(toClassProperty.Name).GetValue(fromClass, null), null);
						}
					}
				}
			}
		}
#nullable enable  
	}
}
