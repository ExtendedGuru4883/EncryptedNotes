namespace BusinessLogic.Helpers;

public static class Base64Helper
{
    public static bool IsValidBase64(string str)
    {
        var isValid = true;
        try
        {
            _ = Convert.FromBase64String(str);
        }
        catch (FormatException)
        {
            isValid = false;
        }
        return isValid;
    }
}