using System;
using System.Reflection;

namespace FinalClick.Services
{
    public class RegisterApplicationServicesAttribute : Attribute
    {
        internal static bool IsMethodValid(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(ServicesCollectionBuilder);
        }
    }
}