using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace DataServices
{
    public class TypedTable : Table
    {
        private static readonly Regex ColumnHeaderRegex = new Regex(@"^(.+) \[(.+)\]$");

        public TypedTable(Table table) : base(StripTypesFromHeader(RemoveCommentColumns(table.Header)))
        {
            SetupRows(table);
            ReplacePlaceholders();
            SetupColumnTypes(table.Header);
        }

        public Dictionary<string, Type> ColumnTypes { get; private set; }

        private static IEnumerable<string> RemoveCommentColumns(IEnumerable<string> header)
        {
            return header.Where(h => !h.ToLower().StartsWith("comment"));
        }

        private static string[] StripTypesFromHeader(IEnumerable<string> header)
        {
            return header.Select(h => ColumnHeaderRegex.Match(h).Groups[1].Value).ToArray();
        }

        private void SetupRows(Table table)
        {
            foreach (var row in table.Rows)
            {
                var rowWithoutComments = row.Where(kvp => !kvp.Key.ToLower().StartsWith("comment"))
                    .Select(kvp => kvp.Value).ToArray();
                AddRow(rowWithoutComments);
            }
        }

        private void ReplacePlaceholders()
        {
            ParsePlaceholders parsePlaceholders = new ParsePlaceholders();
            foreach (var row in Rows)
            foreach (var header in row.Keys)
                row[header] = parsePlaceholders.ParsePlaceholderExpressions(row[header]);
        }



        private void SetupColumnTypes(IEnumerable<string> tableHeader)
        {
            ColumnTypes = new Dictionary<string, Type>();
            foreach (var columnHeader in RemoveCommentColumns(tableHeader))
            {
                var regexMatchGroups = ColumnHeaderRegex.Match(columnHeader).Groups;
                if (regexMatchGroups.Count != 3)
                    throw new ArgumentException(
                        $"Unable to extract column name and type from {columnHeader}. Ensure there is a space between the column and the type");

                var columnName = regexMatchGroups[1].Value;
                var columnType = regexMatchGroups[2].Value;

                ColumnTypes[columnName] = TranslateType(columnType, columnName);
            }
        }

        private static Type TranslateType(string type, string columnName)
        {
            switch (type.ToLower())
            {
                case "varchar":
                case "nvarchar":
                case "string":
                case "nchar":
                case "char":
                    return typeof(string);
                case "bit":
                case "bool":
                    return typeof(bool);
                case "smallint":
                    return typeof(short);
                case "int":
                case "int32":
                    return typeof(int);
                case "bigint":
                case "long":
                case "int64":
                    return typeof(long);
                case "decimal":
                    return typeof(decimal);
                case "float":
                case "double":
                    return typeof(double);
                case "time":
                    return typeof(TimeSpan);
                case "date":
                case "datetime":
                    return typeof(DateTime);
                case "timestamp":
                    return typeof(DateTimeOffset);
                case "customdatecomparison":
                    return typeof(CustomDateComparison);
                default:
                    throw new ArgumentException(
                        $"No translation for column '{columnName}' with '{type}' to an actual type has been setup for the TypedTable");
            }
        }

        public IEnumerable<Dictionary<string, object>> ToDictionaries()
        {
            return Rows.Select(row => Header.ToDictionary(header => header, header => ConvertToType(row, header)));
        }

        public static IEnumerable<Dictionary<string, object>> ToDictionaries(DataTable table)
        {

            return table.AsEnumerable().Select(
                // ...then iterate through the columns...
                row => table.Columns.Cast<DataColumn>().ToDictionary(
                    // ...and find the key value pairs for the dictionary
                    column => column.ColumnName, // Key
                    column => row[column] != DBNull.Value ? row[column] : null  // Value
                )
            );
        }

        private object ConvertToType(TableRow row, string header)
        {
            var value = row[header];
            try
            {
                if (value == null) return null;

                var type = ColumnTypes[header];

                if (type == typeof(bool))
                    switch (value)
                    {
                        case "1":
                            return true;
                        case "0":
                            return false;
                        default:
                            return bool.Parse(value);
                    }

                if (type == typeof(TimeSpan))
                    return TimeSpan.Parse(value);
                if (type == typeof(DateTimeOffset))
                    return DateTimeOffset.Parse(value);
                if (type == typeof(CustomDateComparison))
                    return value;
                return Convert.ChangeType(value, type);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error for column '{header}' datatype and value {value}", ex);
            }
        }
    }

    public class CustomDateComparison
    {
    }
}