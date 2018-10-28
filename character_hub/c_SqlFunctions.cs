using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace character_hub
{
	public class c_SqlFunctions
	{

		/// <summary>SQLite Connection to be used with the functions</summary>
		public SQLiteConnection sqlc { get; set; }
		private List<int> connectedList = new List<int> { 1, 4, 8 };


		/// <summary>Creates the database file if it doesn't already exist</summary>
		/// <param name="dbname">Name/path of the file to be created</param>
		/// <returns>true if file was created</returns>
		public bool createDbFile(string dbname = "db.sqlite")
		{
			if (File.Exists(dbname)) { return true; }

			try
			{
				File.Create(dbname).Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Database file already exists!\r\n" + ex.ToString());
				return false;
			}

			return true;
		}
		
		/// <summary>Creates and opens connection with DB file</summary>
		/// <param name="dbname">Name/path of the file to be connected to</param>
		/// <returns>SQLiteConnection if connected, Null if not</returns>
		public SQLiteConnection connectToDB(string dbname = "db.sqlite")
		{
			SQLiteConnection sql = null;

			try
			{
				sql = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
				sql.Open();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			return sql;
		}
		
		/// <summary>Creates and opens connection with DB file. | Does not return connection, but stores it in this.sqlc</summary>
		/// <param name="dbname">Name/path of the file to be connected to</param>
		/// <returns>true if connection successful</returns>
		public bool createInnerConnection(string dbname = "db.sqlite")
		{
			try
			{
				sqlc = connectToDB(dbname);
				return true;
			}
			catch { return false; }
		}

		/// <summary>Checks the current sqlc connection if it exists and is not closed, connecting or broken</summary>
		/// <returns>true if connection open, false if doesn't exist or not open</returns>
		public bool checkDbConnected()
		{
			return sqlc == null ? false : (connectedList.Contains((int)sqlc.State) ? true : false);
		}

		/// <summary>Checks if a table can be found in the DB already</summary>
		/// <param name="tableName">Name of the table to find</param>
		/// <returns>true if table exists, false if table doesn't exist, null if error</returns>
		public bool? checkTableExists(string tableName)
		{
			if (!checkDbConnected()) { Console.WriteLine("ERROR:SQLC → SQL Connection NULL or not open"); return null; }

			SQLiteCommand sqlk = new SQLiteCommand(string.Format("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{0}';", tableName), sqlc);
			int i = 0;
			try
			{
				int.TryParse(sqlk.ExecuteScalar().ToString(), out i);
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR:SQLC → SQL query returned error: \r\n" + ex.ToString());
				return null;
			}

			return i > 0;
		}

		//TODO: FIX THIS
		/// <summary> Modular table creator function | Creates a new named table if that table doesn't yet exist in the database | Uses modular column definition for reusability</summary>
		/// <param name="tableName">Name of the table to be created</param>
		/// <param name="columns">Array of columns to be added in the newly created table. Type: SQLiteColumn</param>
		/// <returns>true if new table created</returns>
		public bool createTable(string tableName, SQLiteColumn[] columns)
		{
			if (!checkDbConnected()) { Console.WriteLine("ERROR:SQLC → SQL Connection NULL or not open"); return false; }
			object exist = checkTableExists(tableName);
			if (exist == null || (bool)exist) { return false; }

			StringBuilder tableBuilder = new StringBuilder("CREATE TABLE");
			tableBuilder.AppendFormat(" {0} \r\n" +
											"(", tableName);

			foreach (SQLiteColumn c in columns)
			{
				tableBuilder.AppendFormat(" {0} ", c.getColumnDLL());
				if (!columns.Last<SQLiteColumn>().Equals(c)) { tableBuilder.Append(", \r\n"); }
			}

			tableBuilder.Append("\r\n); ");

			if (runNonQuery(tableBuilder.ToString()) == true) { return true; } else { return false; }
		}

		/// <summary> Modular table creator function | Creates a new named table if that table doesn't yet exist in the database | Uses modular column definition for reusability</summary>
		/// <param name="tableName">Name of the table to be created</param>
		/// <param name="columns">List of columns to be added in the newly created table. Type: SQLiteColumn</param>
		/// <returns>true if new table created</returns>
		public bool createTable(string tableName, List<SQLiteColumn> columns)
		{
			SQLiteColumn[] cols = columns.ToArray();
			return createTable(tableName, cols);
		}

		/// <summary>Wrapper for the SQLiteCommand.ExecuteNonQuery function</summary>
		/// <param name="query">Query string to be executed</param>
		/// <returns>true if execution success, false if error</returns>
		public bool runNonQuery(string query)
		{
			if (!checkDbConnected()) { Console.WriteLine("ERROR:SQLC → SQL Connection NULL or not open"); return false; }

			SQLiteCommand sqlk = new SQLiteCommand(query, sqlc);
			int i = -1;

			try
			{
				i = sqlk.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR:SQLC → SQL query returned error: \r\n" + ex.ToString());
				return false;
			}

			return i > -1;
		}

	}

	public static class c_globals
	{
		public static List<SQLiteDataType> typesWithLength = new List<SQLiteDataType> {
			SQLiteDataType.CHARACTER,
			SQLiteDataType.VARCHAR,
			SQLiteDataType.VARYING_CHARACTER,
			SQLiteDataType.NCHAR,
			SQLiteDataType.NATIVE_CHARACTER,
			SQLiteDataType.NVARCHAR
		};
	}

	/// <summary>SQLiteColumn struct, contains all data needed for a basic column definition</summary>
	public class SQLiteColumn
	{
		/// <summary>Name of column</summary>
		public string columnName { get; set; }

		/// <summary>Datatype of column Type: SQLiteDataType</summary>
		public SQLiteDataType dataType { get; set; }

		/// <summary>For certain datatypes a length can be supplied. 
		/// that value can be passed in here. 
		/// If the value does not support length, it will not be used. 
		/// If the data supports length and one is not supplied, default value will be used
		/// </summary>
		public int dataLength { get; set; }

		/// <summary> True if the column is Primary key </summary>
		public bool pimaryKey { get; set; }

		#region FOREIGNKEY

		/// <summary>True if the column is Foreign key</summary>
		public bool foreignKey { get; set; }
		/// <summary>Table name of foreign table</summary>
		public string foreignTable { get; set; }
		/// <summary>Column name of foreign column</summary>
		public string foreignColumn { get; set; }

		#endregion

		/// <summary>True if the column is AutoIncremented</summary>
		public bool autoIncrement { get; set; }

		#region COLUMNCOMMENT

		/// <summary>True if column has comment</summary>
		private bool hasColumnComment;
		/// <summary>Private container for Column Comment</summary>
		private string _columnComment;
		/// <summary>Public container for Column Comment</summary>
		public string columnComment { get { return _columnComment; } set { _columnComment = value; hasColumnComment = value.Length > 0; } }

		#endregion

		/// <summary>Used to write out the PrimaryKey String</summary> <returns>PRIMARY KEY if true empty string otherwise</returns>
		public string pKey() { return pimaryKey ? " PRIMARY KEY " : ""; }

		/// <summary>Used to write out the SecondaryKey String</summary> <returns>SECONDARY KEY if true empty string otherwise</returns>
		public string fKey() { return foreignKey ? " FOREIGN KEY(" + columnName + ") REFERENCES " + foreignTable + "(" + foreignColumn + ") " : ""; }

		/// <summary>Used to write out the SecondaryKey String</summary> <returns>SECONDARY KEY if true empty string otherwise</returns>
		public string aInc() { return autoIncrement ? " AUTOINCREMENT " : ""; }

		/// <summary>Used to write out the column comment if exists</summary> <returns>Comment String if true empty string otherwise</returns>
		public string cCom() { return hasColumnComment ? string.Format(" /* {0} */ ", _columnComment) : ""; }

		/// <summary>Used to write the datatype of the column based on dataType value</summary> <returns>Datatype String</returns>
		public string type()
		{
			string ret = "";
			switch (dataType)
			{
				case SQLiteDataType.NONE:
					ret = "";
					break;

				case SQLiteDataType.INTEGER:
					ret = " INTEGER ";
					break;

				case SQLiteDataType.NUMERIC:
					ret = " NUMERIC ";
					break;

				case SQLiteDataType.REAL:
					ret = " REAL ";
					break;

				case SQLiteDataType.TEXT:
					ret = " TEXT ";
					break;

				//----------------


				case SQLiteDataType.INT:
					ret = " INT ";
					break;

				case SQLiteDataType.TINYINT:
					ret = " TINYINT ";
					break;

				case SQLiteDataType.SMALLINT:
					ret = " SMALLINT ";
					break;

				case SQLiteDataType.MEDIUMINT:
					ret = " MEDIUMINT ";
					break;

				case SQLiteDataType.BIGINT:
					ret = " BIGINT ";
					break;

				case SQLiteDataType.UNSIGNED_BIG_INT:
					ret = " UNSIGNED BIG INT ";
					break;

				case SQLiteDataType.INT2:
					ret = " INT2 ";
					break;

				case SQLiteDataType.INT8:
					ret = " INT8 ";
					break;

				case SQLiteDataType.CHARACTER:
					ret = " CHARACTER ";
					dataLength = ((dataLength == 0) ? 35 : dataLength);
					break;

				case SQLiteDataType.VARYING_CHARACTER:
					ret = " VARYING CHARACTER ";
					dataLength = ((dataLength == 0) ? 120 : dataLength);
					break;

				case SQLiteDataType.NCHAR:
					ret = " NCHAR ";
					dataLength = ((dataLength == 0) ? 35 : dataLength);
					break;

				case SQLiteDataType.NATIVE_CHARACTER:
					ret = " NATIVE CHARACTER ";
					dataLength = ((dataLength == 0) ? 50 : dataLength);
					break;

				case SQLiteDataType.NVARCHAR:
					ret = " NVARCHAR ";
					dataLength = ((dataLength == 0) ? 120 : dataLength);
					break;

				case SQLiteDataType.VARCHAR:
					ret = " VARCHAR ";
					dataLength = ((dataLength == 0) ? 50 : dataLength);
					break;

				case SQLiteDataType.CLOB:
					ret = " CLOB ";
					break;

				case SQLiteDataType.DOUBLE:
					ret = " DOUBLE ";
					break;

				case SQLiteDataType.DOUBLE_PRECISION:
					ret = " DOUBLE PRECISION ";
					break;

				case SQLiteDataType.FLOAT:
					ret = " FLOAT ";
					break;

				case SQLiteDataType.DECIMAL:
					ret = " DECIMAL ";
					break;

				case SQLiteDataType.BOOLEAN:
					ret = " BOOLEAN ";
					break;

				case SQLiteDataType.DATE:
					ret = " DATE ";
					break;

				case SQLiteDataType.DATETIME:
					ret = " DATETIME ";
					break;

				default:
					ret = " TEXT ";
					break;
			}

			ret += (c_globals.typesWithLength.Contains(dataType) ? "(" + dataLength + ")" : "");

			return ret;
		}

		/// <summary>
		/// Gets the DLL for the current column in text format
		/// </summary>
		/// <returns>DLL of column</returns>
		public string getColumnDLL()
		{
			return string.Format(" Column name: {0} \r\nData type: {1} \r\n {2} {3} ",
				columnName,
				type(),
				(pKey() + fKey() + aInc() == "" ? "" : "nModifiers" + pKey() + fKey() + aInc() + "\r\n"),
				cCom() == "" ? "" : cCom() + "\r\n");
		}
	}

	public class SQLTableContainer
	{
		public string name { get; set; }
		public SQLiteColumn[] columns { get; set; }
	}

	/// <summary>Currently supported datatypes | these are the main data container types | Additional types may be added</summary>
	public enum SQLiteDataType
	{
		NONE,
		INTEGER,
		TEXT,
		REAL,
		NUMERIC,
		//-------
		INT,
		TINYINT,
		SMALLINT,
		MEDIUMINT,
		BIGINT,
		UNSIGNED_BIG_INT,
		INT2,
		INT8,
		CHARACTER,
		VARCHAR,
		VARYING_CHARACTER,
		NCHAR,
		NATIVE_CHARACTER,
		NVARCHAR,
		CLOB,
		DOUBLE,
		DOUBLE_PRECISION,
		FLOAT,
		DECIMAL,
		BOOLEAN,
		DATE,
		DATETIME
	}
}
