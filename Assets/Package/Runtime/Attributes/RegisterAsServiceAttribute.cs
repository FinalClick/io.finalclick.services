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

        public RegisterAsServiceAttribute(Type type, params Type[] registerTypes)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            
            if (registerTypes == null || registerTypes.Length == 0)
            {
                RegisterTypes = new[] {type};
                return;
            }
            
            var allRegisterTypes = new List<Type>(registerTypes.Length + 1);
            
            allRegisterTypes.Add(type);
            allRegisterTypes.AddRange(registerTypes);
            
            RegisterTypes = allRegisterTypes.ToArray();


            foreach (var registerType in RegisterTypes)
            {
                if (registerTypes == null)
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