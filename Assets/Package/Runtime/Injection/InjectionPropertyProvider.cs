using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FinalClick.Services.Injection
{
    public static class InjectionPropertyProvider
    {
        private const BindingFlags RequiredBindingFlagsForInjectables = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static IReadOnlyCollection<PropertyInfo> GetInjectableProperties(object injectInto)
        {
            List<PropertyInfo> propertiesWithInjectAttribute = new();

            Type type = injectInto.GetType();

            while (type != null)
            {
                propertiesWithInjectAttribute.AddRange(GetInjectablePropertiesOfType(type));
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    propertiesWithInjectAttribute.AddRange(GetInjectablePropertiesOfType(interfaceType));
                }
                type = type.BaseType;
            }

            return propertiesWithInjectAttribute;
        }

        public static FieldInfo GetBackingField(object obj, PropertyInfo propertyInfo)
        {
            string name = GetBackingFieldName(propertyInfo.Name);

            Type type = obj.GetType();

            while (type != null)
            {
                FieldInfo info = GetBackingFieldFromType(type, name);

                if (info != null)
                {
                    return info;
                }

                type = type.BaseType;
            }

            throw new MissingFieldException(
                $"Unable to find backing field for property '{propertyInfo.Name}' on  '{obj.GetType()}'");
        }

        private static PropertyInfo[] GetInjectablePropertiesOfType(Type type)
        {
            PropertyInfo[] propertiesWithInjectAttribute =
                type.GetProperties(RequiredBindingFlagsForInjectables);
            return propertiesWithInjectAttribute.Where(DoesPropertyHaveInjectAttribute).Where(EnsurePropertyIsInjectable).ToArray();
        }

        private static bool DoesPropertyHaveInjectAttribute(PropertyInfo propertyInfo)
        {
            return Attribute.IsDefined(propertyInfo, typeof(InjectServiceAttribute), true) == true;
        }
        
        private static bool EnsurePropertyIsInjectable(PropertyInfo propertyInfo)
        {
            if (IsPropertyInjectable(propertyInfo) == false)
            {
                throw new ArgumentException($"'{propertyInfo.Name}' on '{propertyInfo.DeclaringType}' is not injectable.");
            }

            return true;
        }
        
        private static bool IsPropertyInjectable(PropertyInfo propertyInfo) 
        {
            if(DoesPropertyHaveInjectAttribute(propertyInfo) == false)
            {
                return false;
            }

            return propertyInfo.CanWrite || HasBackingField(propertyInfo);
        }
        
        private static bool HasBackingField(PropertyInfo propertyInfo)
        {
            var backingFieldName = GetBackingFieldName(propertyInfo.Name);
            return propertyInfo.DeclaringType
                ?.GetField(backingFieldName, RequiredBindingFlagsForInjectables) != null;
        }
        
        
        private static string GetBackingFieldName(string propertyName)
        {
            return $"<{propertyName}>k__BackingField";
        }

        private static FieldInfo GetBackingFieldFromType(Type type, string name)
        {
            return type.GetField(name, RequiredBindingFlagsForInjectables);
        }
    }
}