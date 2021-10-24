using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NUnit.Framework;

namespace DataServices
{
    public class Database
    {
        private const int CommandTimeout = 180;

        protected readonly SqlConnection Connection;

        public Database()
        {
            Connection = new SqlConnection("Data Source=localhost;Initial Catalog=DataservicesTest;user=sa;password=P@ssw0rd");
        }

        public void Insert(string tableName, IEnumerable<Dictionary<string, object>> rows)
        {
            foreach (var row in rows)
            {
                var columns = string.Join(",", row.Keys);
                var values = string.Join(",", row.Keys.Select(k => $"@{k}"));
                var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                try
                {
                    Connection.Execute(sql, row);
                }
                catch
                {
                    TestContext.Progress.WriteLine($"Insert SQL Error Table {tableName}: {sql}");
                    throw;
                }

            }
        }

        public void Update(string tableName, string whereClause, string updateColumnName, string updateValue)
        {
            var sql = $"UPDATE T SET T.{updateColumnName} = {updateValue} FROM {tableName} AS T WHERE {whereClause}";
            try
            {
                Connection.Execute(sql);
            }
            catch
            {
                TestContext.Progress.WriteLine($"Update SQL Error: {sql}");
                throw;
            }
        }


        public IEnumerable<IDictionary<string, object>> ReadAll(string tableName)
        {
            return Connection.Query($"SELECT * FROM {tableName}").Select(d => (IDictionary<string, object>)d);
        }

        public IEnumerable<IDictionary<string, object>> ReadAll(string functionName, Dictionary<string, object> parameters)
        {
            string paramString = String.Join(",", parameters.Values.Select(s => "'" + s + "'"));
            return Connection.Query($"SELECT * FROM {functionName}({paramString});").Select(d => (IDictionary<string, object>)d);
        }

        public IEnumerable<IDictionary<string, object>> Aggregate(string columnName, string tableName, string groupBy)
        {
            string SQLquery = "SELECT " + groupBy + ", SUM(" + columnName + ") as " + columnName + " FROM " + tableName + " GROUP BY " + groupBy;
            return Connection.Query($"{SQLquery}").Select(d => (IDictionary<string, object>)d);
        }

        public IEnumerable<IDictionary<string, object>> ReadDistinctSelectedColumns(string tableName, string commaSeparatedString)
        {
            return Connection.Query($"SELECT DISTINCT {commaSeparatedString} FROM {tableName}").Select(d => (IDictionary<string, object>)d);
        }

        public IEnumerable<IDictionary<string, object>> ReadFiltered(string tableName, string whereClause)
        {
            return Connection.Query($"SELECT * FROM {tableName} WHERE {whereClause}").Select(d => (IDictionary<string, object>)d);
        }


        public IEnumerable<IDictionary<string, object>> ReadColumnDefinitions(string viewTableName, string viewTableSchema, string database = null)
        {
            var databaseUseStatement = database != null ? $"USE {database};" : "";

            return Connection.Query($@"{databaseUseStatement}
            select
                ColumnName = c.COLUMN_NAME,
                SqlType = c.DATA_TYPE
            from INFORMATION_SCHEMA.COLUMNS as c
            where
                c.TABLE_NAME = '{viewTableName}'
                and c.TABLE_SCHEMA = '{viewTableSchema}'").Select(d => (IDictionary<string, object>)d);
        }

        public IEnumerable<IDictionary<string, object>> ReadColumnDetails(string tableName, IEnumerable<string> columnsToIgnore = null)
        {
            string exclusionClause;
            if (columnsToIgnore != null)
            {
                var columnsToIgnoreString = string.Join(",", columnsToIgnore.Select(col => $"'{col}'"));
                exclusionClause = $"AND COLUMN_NAME NOT IN ({columnsToIgnoreString})";
            }
            else
            {
                exclusionClause = "";
            }

            var sql = $@"SELECT LOWER(COLUMN_NAME), DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, DATETIME_PRECISION
                         FROM [INFORMATION_SCHEMA].[COLUMNS]
                         WHERE CONCAT(TABLE_SCHEMA, '.', TABLE_NAME) = '{tableName}'
                         {exclusionClause}
                         ORDER BY COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, DATETIME_PRECISION";
            var columnDetails = Connection.Query(sql).Select(d => (IDictionary<string, object>)d).ToList();

            if (columnDetails.Count == 0) throw new ArgumentException($"No schema details found for {tableName}");

            return columnDetails;
        }

        public void Truncate(string tableName)
        {
            try
            {
                Connection.Execute($"TRUNCATE TABLE {tableName}");
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Message.Contains("because it is not a table"))
                    Connection.Execute($"DELETE FROM {tableName}");
                else
                    throw;
            }
        }

        public void ExecProc(string procName)
        {
            Connection.Execute($"EXEC {procName}");
        }

        public void ExecProc(string procName, IDictionary<string, object> parameters)
        {
            Connection.Execute(procName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout);
        }

        public IEnumerable<IDictionary<string, object>> ExecProcWithResults(string procName, IDictionary<string, object> parameters)
        {
            var results = new List<IDictionary<string, object>>();
            using (var reader = Connection.ExecuteReader(procName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout))
            {
                while (reader.Read())
                {
                    results.Add(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                }
            }

            return results;
        }

        public IEnumerable<IDictionary<string, object>> ExecProcWithResults(string procName)
        {
            var results = new List<IDictionary<string, object>>();
            using (var reader = Connection.ExecuteReader(procName, commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout))
            {
                while (reader.Read())
                {
                    results.Add(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                }
            }

            return results;
        }



    }
}

