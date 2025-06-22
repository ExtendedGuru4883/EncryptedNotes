using System.Text;

namespace Client.Helpers;

public static class QueryStringHelper
{
    public static string ToQueryString(object obj)
    {
        var queryString = new StringBuilder();
        foreach (var property in obj.GetType().GetProperties())
        {
            var propertyValue = property.GetValue(obj);
            if (propertyValue is null) continue;

            var propertyValueString = propertyValue.ToString();
            if (string.IsNullOrEmpty(propertyValueString)) continue;

            queryString.Append($"{Uri.EscapeDataString(property.Name)}={Uri.EscapeDataString(propertyValueString)}&");
        }

        if (queryString.Length > 0)
            queryString.Remove(queryString.Length - 1, 1);

        return queryString.ToString();
    }
}