using System;
using System.Collections.Generic;

namespace FinalClick.Services
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
    }
}