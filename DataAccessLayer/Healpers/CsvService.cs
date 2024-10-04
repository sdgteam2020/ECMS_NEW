using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Healpers
{
    public class CsvService
    {
        public string GenerateCsv<T>(IEnumerable<T> data)
        {
            var properties = typeof(T).GetProperties();  // Get properties of the object
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

            string stringValue = value.ToString();

            // Escape quotes by doubling them, wrap values that contain commas or quotes in quotes
            if (stringValue.Contains(",") || stringValue.Contains("\""))
            {
                stringValue = $"\"{stringValue.Replace("\"", "\"\"")}\"";
            }

            return stringValue;
        }
    }
}
