using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BudgetApp.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value)
        {
            tempData[key] = JsonSerializer.Serialize(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key)
        {
            object value;
            tempData.TryGetValue(key, out value);

            if (value == null)
                return default;

            var result = JsonSerializer.Deserialize<T>((string)value);
            if (result == null && !typeof(T).IsValueType)
                throw new InvalidOperationException(
                    $"Deserialization of key '{key}' failed or returned null."
                );

            return result;
        }
    }
}
