using CsvHelper.Configuration;
using System.Reflection;

namespace Web.Healpers
{
    public class CsvClassMap<T> : ClassMap<T>
    {
        public CsvClassMap(bool isSample, CsvClassMapTypeEnum csvClassMapTypeEnum = CsvClassMapTypeEnum.CardPrint) {
            AutoMap(System.Globalization.CultureInfo.InvariantCulture);

            List<string> ignoreProperties = new List<string>();
            if (csvClassMapTypeEnum == CsvClassMapTypeEnum.CardPrint)
            {
                ignoreProperties.Add("IsValid");
                if (isSample)
                {
                    ignoreProperties.Add("Remarks");
                    ignoreProperties.Add("Status");
                }
            }
            else if (csvClassMapTypeEnum == CsvClassMapTypeEnum.HotlistExport || csvClassMapTypeEnum == CsvClassMapTypeEnum.LostCard || csvClassMapTypeEnum == CsvClassMapTypeEnum.DistributeCard)
            {
                ignoreProperties.Add("RankAbbreviation");
                ignoreProperties.Add("FName");
                ignoreProperties.Add("LName");
                ignoreProperties.Add("IsActiveBool");
                ignoreProperties.Add("ArmyNo");
                ignoreProperties.Add("RankAndName");
                ignoreProperties.Add("Unit");
            }
            else if (csvClassMapTypeEnum == CsvClassMapTypeEnum.DispatchCard)
            {
                ignoreProperties.Add("RequestId");
                ignoreProperties.Add("IsValid");
                ignoreProperties.Add("Remarks");
                ignoreProperties.Add("Status");
            }
            else if (csvClassMapTypeEnum == CsvClassMapTypeEnum.CSVExport)
            {

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
