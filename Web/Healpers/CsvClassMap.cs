using CsvHelper.Configuration;
using System.Reflection;

namespace Web.Healpers
{
    public class CsvClassMap<T> : ClassMap<T>
    {
        public CsvClassMap(bool isSample) {
            AutoMap(System.Globalization.CultureInfo.InvariantCulture);

            List<string> ignoreProperties = new List<string>();
            ignoreProperties.Add("IsValid");
            if (isSample)
            {
                ignoreProperties.Add("Remarks");
                ignoreProperties.Add("Status");
            }
            foreach (var prop in typeof(T).GetProperties())
            {
                if (ignoreProperties.Contains(prop.Name))
                {
                    Map(typeof(T), prop).Ignore();
                }
            }
        }
    }
}
