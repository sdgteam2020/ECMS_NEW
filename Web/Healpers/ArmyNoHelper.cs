using DataTransferObject.Domain.Master;
using System.Text.RegularExpressions;

namespace Web.Healpers
{
    public static class ArmyNoHelper
    {
        public static string ValidateArmyNo(string? armyNo, List<MArmyPrefixRule> mArmyPrefixes)
        {
            // Check empty or null
            if (string.IsNullOrWhiteSpace(armyNo))
                return "Army No is required.";

            armyNo = armyNo.Trim().ToUpperInvariant();

            // Minimum: 1 prefix + 5 digits + 1 suffix = 7
            // Maximum: 2 prefix + 6 digits + 1 suffix = 9
            // Or: 3 prefix + 5 digits + 1 suffix = 9
            if (armyNo.Length < 7 || armyNo.Length > 9)
                return "Invalid Army No Length.";

            // Prefix: 1 to 3 alphabets
            // Number: 5 or 6 digits
            // Suffix: exactly 1 alphabet
            Match match = Regex.Match(armyNo,@"^([A-Z]{1,3})([0-9]{5,6})([A-Z])$");

            if (!match.Success)
                return "Invalid Army No Format.";

            // Extract components
            string prefix = match.Groups[1].Value;
            string numericPart = match.Groups[2].Value;
            string actualSuffix = match.Groups[3].Value;

            if (mArmyPrefixes.Count > 0)
            {
                // Validate prefix against allowed list
                if (!mArmyPrefixes.Any(x =>
                    string.Equals(x.Prefix?.Trim(), prefix, StringComparison.OrdinalIgnoreCase)))
                    return "Invalid Army No Prefix.";
            }


            // Three-letter prefixes require exactly 5 digits
            if (prefix.Length == 3 && numericPart.Length != 5)
                return "Three-letter Army No Prefix requires exactly 5 digits.";

            // Calculate expected suffix using Modulo 11
            int length = numericPart.Length;
            int multiplier = length + 1;
            int sum = 0;

            for (int i = 0; i < length; i++)
            {
                int digit = numericPart[i] - '0';

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

            // Validate suffix
            if (actualSuffix != expectedSuffix)
                return "Invalid Army No Suffix.";

            // Valid Army Number
            return string.Empty;
        }
    }
}
