using System.Text.RegularExpressions;

namespace Web.Healpers
{
    public static class ArmyNoHelper
    {
        private static readonly HashSet<string> ValidPrefixes = new HashSet<string>
        {
            "IC", "SL", "SS", "WC", "TA"
        };

        public static string ValidateArmyNo(string? armyNo)
        {
            if (string.IsNullOrWhiteSpace(armyNo))
                return "Army No is required.";

            armyNo = armyNo.Trim().ToUpper();

            if (armyNo.Length < 8 || armyNo.Length > 9)
                return "Invalid Army No.";

            if (!Regex.IsMatch(armyNo, @"^[A-Z]{2}\d{5,6}[A-Z]$"))
                return "Invalid Army No.";

            string prefix = armyNo.Substring(0, 2);
            if (!ValidPrefixes.Contains(prefix))
                return "Invalid Army No.";

            string numericPart = Regex.Replace(armyNo, "[A-Za-z]", "");
            string actualSuffix = armyNo[^1].ToString();

            int length = numericPart.Length;
            int multiplier = length + 1;
            int sum = 0;

            for (int i = 0; i < length; i++)
            {
                int digit = int.Parse(numericPart[i].ToString());
                sum += digit * multiplier;
                multiplier--;
            }

            int remainder = sum % 11;

            string expectedSuffix = remainder switch
            {
                0 => "A",
                1 => "F",
                2 => "H",
                3 => "K",
                4 => "L",
                5 => "M",
                6 => "N",
                7 => "P",
                8 => "W",
                9 => "X",
                10 => "Y",
                _ => ""
            };

            if (actualSuffix != expectedSuffix)
                return "Invalid Army No.";

            return string.Empty;
        }
    }
}
