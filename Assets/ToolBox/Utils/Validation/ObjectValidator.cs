using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ToolBox.Utils.Validation
{
    public static class ObjectValidator
    {
        private static readonly Dictionary<Type, List<MemberInfo>> CachedMembers = new();

        /// <summary>
        /// Validates the specified object by ensuring its marked fields and properties are set.
        /// </summary>
        /// <typeparam name="T">The type of the object being validated.</typeparam>
        /// <param name="target">The object to validate.</param>
        /// <param name="context">The Unity Object for better log context (optional, e.g., GameObject or Component).</param>
        /// <param name="haltOnError">If true, pauses the editor on critical failures (only in editor).</param>
        /// <returns>True if all validations pass; false otherwise.</returns>
        public static bool Validate<T>(T target, UnityEngine.Object context = null, bool haltOnError = false)
        {
            if (target == null)
            {
                Logger.LogError($"Validation failed: The target object of type {typeof(T).Name} is null.");
                if (haltOnError && Application.isEditor) Debug.Break();
                return false;
            }

            Type type = target.GetType();

            // Check if members are already cached for this type
            if (!CachedMembers.TryGetValue(type, out var members))
            {
                members = new List<MemberInfo>();

                // Cache fields with the Validate attribute
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (Attribute.IsDefined(field, typeof(ValidateAttribute)))
                        members.Add(field);
                }

                // Cache properties with the Validate attribute
                foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (Attribute.IsDefined(property, typeof(ValidateAttribute)) && property.CanRead)
                        members.Add(property);
                }

                // Cache the members for future validations
                CachedMembers[type] = members;
            }

            bool isValid = true;

            foreach (var member in members)
            {
                object value = member is FieldInfo field
                    ? field.GetValue(target)
                    : ((PropertyInfo)member).GetValue(target);

                // Check for null or destroyed UnityEngine.Object
                if (value == null || (value is UnityEngine.Object unityObject && unityObject == null))
                {
                    isValid = false;

                    string contextName = context != null ? context.name : "Unknown Context";
                    Debug.LogError($"Validation failed: {member.Name} is not set or destroyed on {contextName}.", context);

                    if (haltOnError && Application.isEditor)
                    {
                        Debug.Break();
                        return false;
                    }
                }
            }

            return isValid;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ValidateAttribute : Attribute
    {
    }
}