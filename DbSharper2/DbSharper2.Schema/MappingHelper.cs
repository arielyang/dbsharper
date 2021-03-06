﻿using System;
using System.Configuration;
using System.Globalization;
using System.IO;

using DbSharper2.Schema.Infrastructure;

namespace DbSharper2.Schema
{
	internal static class MappingHelper
	{
		internal static string RemoveNamespace(string typeFullName)
		{
			int index = typeFullName.LastIndexOf('.');

			if (index < 0)
			{
				return typeFullName;
			}

			return typeFullName.Substring(index + 1);
		}

		#region Methods

		/// <summary>
		/// Get
		/// </summary>
		/// <param name="commonType"></param>
		/// <returns></returns>
		internal static string GetCommonTypeString(CommonType commonType)
		{
			switch (commonType)
			{
				case CommonType.ByteArray:
					return "Byte[]";
				case CommonType.CharArray:
					return "Char[]";
				default:
					return commonType.ToString();
			}
		}

		internal static string GetConnectionStringName(string mappingConfigFile)
		{
			return Path.GetFileNameWithoutExtension(mappingConfigFile);
		}

		internal static ConnectionStringSettings GetConnectionStringSettings(string configFile, string connectionStringName)
		{
			if (string.IsNullOrEmpty(configFile))
			{
				throw new ArgumentNullException("configFile");
			}

			if (string.IsNullOrEmpty(connectionStringName))
			{
				throw new ArgumentNullException("connectionStringName");
			}

			if (!File.Exists(configFile))
			{
				// TODO: Embed string into resource file later.
				throw new DbSharperException(string.Format(CultureInfo.InvariantCulture, "Can not find configuration file \"{0}\".", configFile));
			}

			ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();

			fileMap.ExeConfigFilename = configFile;

			System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

			ConnectionStringSettings settings = configuration.ConnectionStrings.ConnectionStrings[connectionStringName];

			if (settings == null)
			{
				// TODO: Embed string into resource file later.
				throw new DbSharperException(string.Format(CultureInfo.InvariantCulture, "Can not find connection string named \"{0}\".", connectionStringName));
			}

			return settings;
		}

		//internal static string GetForeignKeyName(Table table, string columnName)
		//{
		//	var foreignKeys = table.ForeignKeys;
		//	foreach (ForeignKey foreignKey in foreignKeys)
		//	{
		//		if (foreignKey.Columns.Contains(columnName))
		//		{
		//			return foreignKey.Name;
		//		}
		//	}
		//	return null;
		//}
		/// <summary>
		/// Get method type by method name.
		/// </summary>
		/// <param name="methodName">Method name.</param>
		/// <returns>Method type.</returns>
		internal static MethodType GetMethodType(string methodName)
		{
			return methodName.StartsWith("Get", StringComparison.OrdinalIgnoreCase) ? MethodType.ExecuteReader : MethodType.ExecuteNonQuery;
		}

		#endregion Methods

		#region Other

		//internal static string GetPrimaryKeyName(Table table, string columnName)
		//{
		//	if (table.PrimaryKey.Columns.Contains(columnName))
		//	{
		//		return table.PrimaryKey.Name;
		//	}
		//	return null;
		//}

		#endregion Other
	}
}