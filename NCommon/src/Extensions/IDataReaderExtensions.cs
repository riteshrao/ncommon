using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NCommon.Extensions
{
    public static class IDataReaderExtensions
    {

		/// <summary>
		/// Acquires the value out of a row of a IDataReader based on the column name.
		/// </summary>
		/// <param name="dr">A populated IDataReader (DataReader)</param>
		/// <param name="index">The index of the column to retrieve the value from</param>
		/// <param name="defaultValue">The default value of the column should the column not be found.</param>
		/// <returns>The value of the object</returns>
		/// <remarks>If the index is not found, then this method throws an exception and assigns the default value.</remarks>
		public static object GetValue(this IDataReader dr, int index, object defaultValue)
		{
			object rv = defaultValue;

			try
			{
				rv = dr.GetValue(index);

				if (rv == DBNull.Value)
				{
					rv = defaultValue;
				}
			}
			catch (Exception)
			{

				rv = defaultValue;
			}
			return rv;
		}

		public static object GetValue(this IDataReader dr, string columnName, object defaultValue)
		{
			object rv = defaultValue;

			try
			{
				int index = dr.GetOrdinal(columnName);
				rv = dr.GetValue(index);

				if (rv == DBNull.Value)
				{
					rv = defaultValue;
				}
			}
			catch (Exception)
			{

				rv = defaultValue;
			}
			return rv;
		}

		/// <summary>
		/// Converts a DataReader Interface (IDataReader) to a DataTable
		/// </summary>
		/// <param name="dr">Data Reader Interface containing the data</param>
		/// <returns>A populated DataTable</returns>
		/// <remarks>This method does not close the IDataReader.  You will have to.</remarks>
		public static DataTable ToDataTable(this IDataReader dr)
		{
			DataTable dtSchema = dr.GetSchemaTable();
			DataTable dtData = new DataTable();
			DataColumn dc;
			DataRow row;
			System.Collections.ArrayList al = new System.Collections.ArrayList();

			// Populate the Column Information
			for (int i = 0; i < dtSchema.Rows.Count; i++)
			{
				dc = new DataColumn();

				if (!dtData.Columns.Contains(dtSchema.Rows[i]["ColumnName"].ToString()))
				{
					dc.ColumnName = dtSchema.Rows[i]["ColumnName"].ToString();
					dc.Unique = Convert.ToBoolean(dtSchema.Rows[i]["IsUnique"]);
					dc.AllowDBNull = Convert.ToBoolean(dtSchema.Rows[i]["AllowDBNull"]);
					dc.ReadOnly = Convert.ToBoolean(dtSchema.Rows[i]["IsReadOnly"]);
					dc.DataType = (Type)dtSchema.Rows[i]["DataType"];
					al.Add(dc.ColumnName);

					dtData.Columns.Add(dc);
				}
			}

			// Loop through the data
			while (dr.Read())
			{
				row = dtData.NewRow();

				for (int i = 0; i < al.Count; i++)
				{
					row[((System.String)al[i])] = dr[(System.String)al[i]];
				}

				dtData.Rows.Add(row);
			}
			dr.Close();

			return dtData;
		}

		/// <summary>
		/// Converts a DataReader Interface (IDataReader) to a DataTable
		/// </summary>
		/// <param name="dr">Data Reader Interface containing the data</param>
		/// <param name="destroyReader">Determines weather or not to destory the IDataReader after the DataTable has been populated.</param>
		/// <returns>A populated DataTable</returns>
		public static DataTable ToDataTable(this IDataReader dr, bool destroyReader)
		{
			try
			{
				DataTable dtSchema = dr.GetSchemaTable();
				DataTable dtData = new DataTable();
				DataColumn dc;
				DataRow row;
				System.Collections.ArrayList al = new System.Collections.ArrayList();

				// Populate the Column Information
				for (int i = 0; i < dtSchema.Rows.Count; i++)
				{
					dc = new DataColumn();

					if (!dtData.Columns.Contains(dtSchema.Rows[i]["ColumnName"].ToString()))
					{
						dc.ColumnName = dtSchema.Rows[i]["ColumnName"].ToString();
						dc.Unique = Convert.ToBoolean(dtSchema.Rows[i]["IsUnique"]);
						dc.AllowDBNull = Convert.ToBoolean(dtSchema.Rows[i]["AllowDBNull"]);
						dc.ReadOnly = Convert.ToBoolean(dtSchema.Rows[i]["IsReadOnly"]);
						al.Add(dc.ColumnName);
						dtData.Columns.Add(dc);
					}
				}

				// Loop through the data
				while (dr.Read())
				{
					row = dtData.NewRow();

					for (int i = 0; i < al.Count; i++)
					{
						row[((System.String)al[i])] = dr[(System.String)al[i]];
					}

					dtData.Rows.Add(row);
				}



				return dtData;
			}
			catch (Exception ex)
			{

				throw ex;
			}
			finally
			{
				if (destroyReader && !dr.IsClosed)
				{
					dr.Close();
				}
			}
		}
    }
}
