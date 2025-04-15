using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Healpers
{
    public class DataTableHelper
    {
        public static DataTable ToDataTable<T>(T[] data, params string[] columnsToIgnore)
        {
            DataTable dataTable = new DataTable();

            // Get properties of the class
            PropertyInfo[] properties = typeof(T).GetProperties()
                .Where(p => !columnsToIgnore.Contains(p.Name)) // Ignore unwanted columns
                .ToArray();

            // Create DataTable columns based on properties
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Populate rows with object values
            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item, null) ?? DBNull.Value).ToArray();
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
