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

namespace DataLayer
{
	public class Common
	{
		private readonly Logger logger;
		private IConfiguration _configuration;
		private static readonly Lazy<Common> _instance = new Lazy<Common>(() => new Common());
		public static Common Instance => _instance.Value;

		private Common() 
		{
			logger = NLog.LogManager.GetCurrentClassLogger();
		}

		public void Initialize(IConfiguration configuration)
		{
			this._configuration = configuration;
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
