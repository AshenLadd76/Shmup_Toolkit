using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ToolBox.Data.Importer
{
    public static class CsvToScriptableObjectMapper 
    {
        // Populates a ScriptableObject instance of type T with values from the CSV row
        public static void PopulateSoFromCsvRow<T>(T scriptableObject, List<string> headers, List<string> values) where T : ScriptableObject
        {
            var type = typeof(T);
            
            // Get all public instance fields and properties
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                string value = i < values.Count ? values[i] : "";
                
                // Try to find a matching field (case-insensitive)
                var field = Array.Find(fields, f => string.Equals(f.Name, header, StringComparison.OrdinalIgnoreCase));
                if (field != null)
                {
                    object converted = ConvertValue(value, field.FieldType);
                    field.SetValue(scriptableObject, converted);
                    continue;
                }

                // Try to find a matching property (case-insensitive)
                var property = Array.Find(properties, p => string.Equals(p.Name, header, StringComparison.OrdinalIgnoreCase) && p.CanWrite);
                if (property != null)
                {
                    object converted = ConvertValue(value, property.PropertyType);
                    property.SetValue(scriptableObject, converted);
                }
            }

        }
        
        // Converts the string value from CSV into the target field/property type
        private static object ConvertValue(string value, Type targetType)
        {
            if (targetType == typeof(string))
                return value;

            if (string.IsNullOrEmpty(value))
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            try
            {
                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value, ignoreCase: true);

                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                Debug.LogWarning($"Failed to convert '{value}' to {targetType.Name}, using default.");
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }
    }
}
