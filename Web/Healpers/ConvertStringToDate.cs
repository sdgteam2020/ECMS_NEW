using System.Globalization;

namespace Web.Healpers
{
    public class ConvertStringToDate
    {
        public static DateTime? ConvertDob(string string_date)
        {
            if (string.IsNullOrWhiteSpace(string_date))
                return null;

            string[] allowedFormats =
            {
                "dd/MM/yyyy",
                "d/M/yyyy",
                "dd-MM-yyyy",
                "d-M-yyyy"
            };

            bool isValid = DateTime.TryParseExact(
                string_date.Trim(),
                allowedFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime result
            );

            if (!isValid)
                return null;

            return result;
        }
    }
}
