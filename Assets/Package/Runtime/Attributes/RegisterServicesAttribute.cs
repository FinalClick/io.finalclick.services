using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FinalClick.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RegisterServicesAttribute : Attribute
    {
        internal static bool IsMethodValid(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(ServicesCollectionBuilder);
        }
        
        internal static IEnumerable<MethodInfo> GetAllStaticRegisterServicesMethods()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var methods = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                .Where(method => method.GetCustomAttributes(typeof(RegisterServicesAttribute), false).Length > 0);
            
            return methods;
        }
    }
}