namespace Shared.Helpers;

public class SanitizeForLogging
{
    public static string Sanitize(string input)
    {
        return string.IsNullOrEmpty(input) ? input : input.Replace("\r", "").Replace("\n", "").Trim();
    }

}