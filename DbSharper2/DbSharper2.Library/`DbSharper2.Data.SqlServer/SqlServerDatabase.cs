﻿using System.Data.SqlClient;

using DbSharper2.Library.Data;

namespace DbSharper2.Data.SqlServer
{
	public class SqlServerDatabase : Database
	{
		#region Constructors

		public SqlServerDatabase()
			: base(SqlClientFactory.Instance)
		{
		}

		#endregion Constructors

		#region Methods

		public override string BuildParameterName(string name)
		{
			if (name[0] != '@')
			{
				return "@" + name;
			}

			return name;
		}

		#endregion Methods
	}
}