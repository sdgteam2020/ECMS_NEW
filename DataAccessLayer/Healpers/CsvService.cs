using DataTransferObject.Validation;
using System.Text;

namespace DataAccessLayer.Healpers
{
    public class CsvService
    {
        public string GenerateCsv<T>(IEnumerable<T> data)
        {
            var properties = typeof(T)
                .GetProperties()
                .Where(p => !Attribute.IsDefined(p, typeof(CsvIgnoreAttribute)))
                .ToArray();

            var csvBuilder = new StringBuilder();

            // Add the headers (column names) to the CSV
            csvBuilder.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Add the data rows
            foreach (var item in data)
            {
                var row = string.Join(",", properties.Select(p => FormatCsvValue(p.GetValue(item))));
                csvBuilder.AppendLine(row);
            }

            return csvBuilder.ToString();
        }
        // Helper method to format values for CSV (handles commas, quotes, and nulls)
        private string FormatCsvValue(object value)
        {
            if (value == null)
                return "";

            string stringValue = value.ToString().Trim(); // Trim to avoid accidental whitespace issues

            // Remove unintended newlines to prevent CSV row breaks
            stringValue = stringValue.Replace("\r", " ").Replace("\n", " ");

            // Escape double quotes by doubling them (CSV standard)
            stringValue = stringValue.Replace("\"", "\"\"");


            // Replace & with a safe version to prevent breaking rows
            //stringValue = stringValue.Replace("&", "&amp;");

            // Escape quotes by doubling them, wrap values that contain commas or quotes in quotes
            if (stringValue.Contains(",") || stringValue.Contains("\"") || stringValue.Contains("&"))
            {
                //stringValue = $"\"{stringValue.Replace("\"", "\"\"")}\"";
                stringValue = $"\"{stringValue}\"";
            }

            return stringValue;
        }
    }
}
