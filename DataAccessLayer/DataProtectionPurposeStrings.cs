namespace DataAccessLayer
{
    /// <summary>
    /// A class to store string constants used for data protection purposes.
    /// </summary>
    /// <remarks>
    /// This class contains readonly fields that are used as keys or identifiers for data protection scenarios.
    /// For example, the `AFSACIdRouteValue` field is used as an identifier for a specific route value.
    /// These string constants can be used throughout the application to ensure consistency and avoid hard-coding values.
    /// </remarks>
    public class DataProtectionPurposeStrings
    {
        public readonly string AFSACIdRouteValue = "AFSACIdRouteValue";
    }
}
