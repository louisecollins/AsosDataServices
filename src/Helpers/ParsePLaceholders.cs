using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DataServices
{
    class ParsePlaceholders
    {
        public string ParsePlaceholderExpressions(string expr)
        {
            var regex = new Regex(@".*([-+]\d+)([yMdhms]?)");
            string[] timeParts = { "h", "m", "s" };
            var matches = regex.Matches(expr);
            if (expr == "${null}")
                return null;
            if (expr != null && expr.Contains("UtcNowDateKey") && matches.Count == 0)
                return ParseUtcNowDateExpression(expr, "yyyyMMdd"); // Return only the date portion of the current UTC date
            if (expr != null && expr.Contains("UtcNowDateKey") && !timeParts.Any(matches[0].Groups[2].Value.Contains))
                return ParseUtcNowDateExpression(expr, "yyyyMMdd"); // If a date difference is specified, format as date only
            if (expr != null && expr.Contains("UtcNowDateKey") && timeParts.Any(matches[0].Groups[2].Value.Contains))
                return ParseUtcNowDateExpression(expr, "yyyyMMddhhmmss"); // If a time difference is specified, format as datetime
            if (expr != null && expr.Contains("UtcNowDate") && matches.Count == 0)
                return ParseUtcNowDateExpression(expr, "yyyy-MM-dd"); // Return only the date portion of the current UTC date
            if (expr != null && expr.Contains("UtcNowDate") && !timeParts.Any(matches[0].Groups[2].Value.Contains))
                return ParseUtcNowDateExpression(expr, "yyyy-MM-dd"); // If a date difference is specified, format as date only
            if (expr != null && expr.Contains("UtcNowDate") && timeParts.Any(matches[0].Groups[2].Value.Contains))
                return ParseUtcNowDateExpression(expr, "yyyy-MM-dd hh:mm:ss"); // If a time difference is specified, format as datetime
            if (expr != null && expr.Contains("StartOfWeekKey") && matches.Count == 0)
                return ParseStartOfWeekExpression(DateTime.UtcNow, DayOfWeek.Monday, expr, "yyyyMMdd"); // Return only the date portion of the start of week day
            if (expr != null && expr.Contains("StartOfWeekKey") && !timeParts.Any(matches[0].Groups[2].Value.Contains))
                return ParseStartOfWeekExpression(DateTime.UtcNow, DayOfWeek.Monday, expr, "yyyyMMdd"); // If a date difference is specified, format as date only
            if (expr != null && expr.Contains("StartOfWeekKey") && timeParts.Any(matches[0].Groups[2].Value.Contains))
                return ParseStartOfWeekExpression(DateTime.UtcNow, DayOfWeek.Monday, expr, "yyyyMMddhhmmss"); // If a time difference is specified, format as datetime
            if (expr != null && expr.Contains("StartOfWeek") && matches.Count == 0)
                return ParseStartOfWeekExpression(DateTime.UtcNow, DayOfWeek.Monday, expr, "yyyy-MM-dd"); // Return only the date portion of the start of week day
            if (expr != null && expr.Contains("StartOfWeek") && !timeParts.Any(matches[0].Groups[2].Value.Contains))
                return ParseStartOfWeekExpression(DateTime.UtcNow, DayOfWeek.Monday, expr, "yyyy-MM-dd"); // If a date difference is specified, format as date only
            if (expr != null && expr.Contains("StartOfWeek") && timeParts.Any(matches[0].Groups[2].Value.Contains))
                return ParseStartOfWeekExpression(DateTime.UtcNow, DayOfWeek.Monday, expr, "yyyy-MM-dd hh:mm:ss"); // If a time difference is specified, format as datetime
            return expr;
        }

        private static string ParseUtcNowDateExpression(string expr, string format)
        {
            var regex = new Regex(@".*([-+]\d+)([ymdhMs]?)");
            var matches = regex.Matches(expr);
            if (matches.Count == 0) return DateTime.UtcNow.ToString(format);
            var numberOfDateparts = int.Parse(matches[0].Groups[1].Value);
            if (matches[0].Groups[2].Value.Equals("y", StringComparison.Ordinal)) return DateTime.UtcNow.AddYears(numberOfDateparts).ToString(format);
            if (matches[0].Groups[2].Value.Equals("M", StringComparison.Ordinal)) return DateTime.UtcNow.AddMonths(numberOfDateparts).ToString(format);
            if (matches[0].Groups[2].Value.Equals("d", StringComparison.Ordinal)) return DateTime.UtcNow.AddDays(numberOfDateparts).ToString(format);
            // Return current UtcNow at 00:00 + time parts
            if (matches[0].Groups[2].Value.Equals("h", StringComparison.Ordinal)) return DateTime.UtcNow.Date.AddHours(numberOfDateparts).ToString(format);
            if (matches[0].Groups[2].Value.Equals("m", StringComparison.Ordinal)) return DateTime.UtcNow.Date.AddMinutes(numberOfDateparts).ToString(format);
            if (matches[0].Groups[2].Value.Equals("s", StringComparison.Ordinal)) return DateTime.UtcNow.Date.AddSeconds(numberOfDateparts).ToString(format);
            return DateTime.UtcNow.AddDays(numberOfDateparts).ToString(format);
        }

        private static string ParseStartOfWeekExpression(DateTime dt, DayOfWeek startOfWeekDay, string expr, string format)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeekDay)) % 7;
            DateTime startOfWeek = dt.AddDays(-1 * diff).Date;
            var regex = new Regex(@".*([-+]\d+)([ymdhMs]?)");
            var matches = regex.Matches(expr);
            if (matches.Count == 0) return startOfWeek.ToString(format);
            var numberOfDateparts = int.Parse(matches[0].Groups[1].Value);
            if (matches[0].Groups[2].Value.Equals("y", StringComparison.Ordinal)) return startOfWeek.AddYears(numberOfDateparts).ToString(format);
            if (matches[0].Groups[2].Value.Equals("M", StringComparison.Ordinal)) return startOfWeek.AddMonths(numberOfDateparts).ToString(format);
            if (matches[0].Groups[2].Value.Equals("d", StringComparison.Ordinal)) return startOfWeek.AddDays(numberOfDateparts).ToString(format);
            // Return start of week at 00:00 + time parts
            if (matches[0].Groups[2].Value.Equals("h", StringComparison.Ordinal)) return startOfWeek.AddHours(numberOfDateparts).ToString(format);
            if (matches[0].Groups[2].Value.Equals("m", StringComparison.Ordinal)) return startOfWeek.AddMinutes(numberOfDateparts).ToString(format);
            if (matches[0].Groups[2].Value.Equals("s", StringComparison.Ordinal)) return startOfWeek.AddSeconds(numberOfDateparts).ToString(format);
            return DateTime.UtcNow.AddDays(numberOfDateparts).ToString(format);
        }
    }
}
