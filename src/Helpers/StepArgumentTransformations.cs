using System;
using System.Linq;
using TechTalk.SpecFlow;
using System.Text.RegularExpressions;

namespace DataServices
{
    [Binding]
    public class StepArgumentTransformations
    {
        [StepArgumentTransformation]
        public TypedTable TableToTypedTable(Table table)
        {
            return new TypedTable(table);
        }

        [StepArgumentTransformation]
        public Table RemoveCommentColumns(Table table)
        {
            var headersWithoutComments = table.Header.Where(h => !h.ToLower().StartsWith("comment")).ToArray();
            var newTable = new Table(headersWithoutComments);
            foreach (var tableRow in table.Rows)
            {
                var cleanedRow = tableRow.Where(kvp => !kvp.Key.ToLower().StartsWith("comment")).Select(kvp => kvp.Value).ToArray();
                newTable.AddRow(cleanedRow);
            }

            return newTable;
        }

    }
}