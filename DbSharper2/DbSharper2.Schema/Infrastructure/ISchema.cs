﻿namespace DbSharper2.Schema.Infrastructure
{
	public interface ISchema : IName
	{
		#region Properties

		string Description
		{
			get;
			set;
		}

		string Schema
		{
			get; set;
		}

		#endregion Properties
	}
}