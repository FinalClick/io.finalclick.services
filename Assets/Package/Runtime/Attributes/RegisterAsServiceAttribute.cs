using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FinalClick.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsServiceAttribute : Attribute
    {
        public readonly Type[] RegisterTypes;
        public bool RegisterSelfAsServiceType => RegisterTypes.Length == 0;

        public RegisterAsServiceAttribute(params Type[] registerTypes)
        {
            RegisterTypes = registerTypes ?? Array.Empty<Type>();
            
            foreach (var registerType in RegisterTypes)
            {
                if (registerType == null)
                {
                    throw new ArgumentNullException(nameof(registerTypes));
                }
            }
        }
        
        internal static IEnumerable<MethodInfo> GetAllAutoRegisterServicesInstanceMethods(MonoBehaviour monoBehaviour)
        {
            // Can be null if "The references scripted on this Behaviour (Unknown) is missing!" warnings.
            if (monoBehaviour == null)
            {
                return Array.Empty<MethodInfo>();
            }
            var methods = monoBehaviour.GetType()
                .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                .Where(method => method.GetCustomAttributes(typeof(RegisterServicesAttribute), false).Length > 0);
            
            return methods;
        }

        
    }
}