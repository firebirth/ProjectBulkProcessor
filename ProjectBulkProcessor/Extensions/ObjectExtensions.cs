using System.Collections.Generic;
using System.Reflection;

namespace ProjectBulkProcessor.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<(string Name, string Value)> ToNameValuePair<T>(this T value)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(value);
                if (propertyValue != null)
                {
                    yield return (Name: property.Name, Value: propertyValue.ToString());
                }
            }
        }
    }
}
