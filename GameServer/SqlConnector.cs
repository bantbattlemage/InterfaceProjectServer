using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using GameComms;

namespace GameServer
{
	public static class SqlConnector
	{
		private static SqlConnectionStringBuilder _builder;

		public static string ToSqlTime(this DateTime t)
		{
			return t.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static T[] ExtractListFromResult<T>(this JsonResult query)
		{
			T[] output = new T[0];

			if (query == null || (string)query.Value == "")
			{
				output = new T[0];
			}
			else
			{
				try
				{
					output = JsonConvert.DeserializeObject<T[]>((string)query.Value);
				}
				catch
				{
					output = new T[] { JsonConvert.DeserializeObject<T>((string)query.Value) };
				}
			}

			return output;
		}

		public static SqlConnectionStringBuilder Builder(string catalog = "TestDatabase")
		{
			if (_builder != null)
			{
				return _builder;
			}

			_builder = new SqlConnectionStringBuilder(Startup.ConnectionString);

			return _builder;
		}

		public static JsonResult Query(string query = "", string catalog = "TestDatabase", Action<JsonResult> callback = null)
		{
			SqlConnectionStringBuilder builder = Builder(catalog);

			try
			{
				Console.Write("Connecting to SQL Server ... ");

				using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
				{
					connection.Open();

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						string json = "";

						using (SqlDataReader reader = command.ExecuteReader())
						{
							int readCount = 0;

							while (reader.Read())
							{
								string read = "{";

								for (int i = 0; i < reader.FieldCount; i++)
								{
									string field = "";
									field += reader.GetName(i) + ":";
									Type fieldType = reader.GetFieldType(i);
									var value = reader.GetValue(i);

									if(value is string || value is DateTime)
									{
										field += $"'{value}'";
									}
									else
									{
										field += $"{value}";
									}

									if (i < reader.FieldCount - 1)
									{
										field += ",";
									}

									read += field;
								}

								read += "},";
								json += read;
								readCount++;
							}

							if (json != "")
							{
								//	remove last comma
								json = json.Remove(json.Length - 1, 1);

								//	add backets if result is an array
								if (readCount > 1)
								{
									json = json.Insert(0, "[");
									json += "]";
								}
							}

							Console.WriteLine(json);

							JsonResult result = new JsonResult(json);
							if (callback != null)
							{
								callback(result);
							}

							return result;
						}
					}
				}
			}
			catch (SqlException e)
			{
				Console.WriteLine(e.ToString());
				return null;
			}
		}
	}
}