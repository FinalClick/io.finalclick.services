using System;
using System.Reflection;

namespace FinalClick.Services
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RegisterServicesAttribute : Attribute
    {
        internal static bool IsMethodValid(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(ServicesCollectionBuilder);
        }
    }
}