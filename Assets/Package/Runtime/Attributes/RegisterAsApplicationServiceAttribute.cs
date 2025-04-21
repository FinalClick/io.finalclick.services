using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalClick.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RegisterAsApplicationServiceAttribute : Attribute
    {
        public readonly Type[] RegisterTypes;
        public bool RegisterSelfAsServiceType => RegisterTypes.Length == 0;

        public RegisterAsApplicationServiceAttribute(params Type[] registerTypes)
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
        
        public static IEnumerable<Type> GetTypesWithApplicationServiceAttribute()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.
                SelectMany(x => x.GetTypes())
                .Where(t => t.IsDefined(typeof(RegisterAsApplicationServiceAttribute), inherit: false)).ToArray();

            foreach (var type in types)
            {
                AssertValidType(type);
            }
            
            return types;
        }


        private static void AssertValidType(Type type)
        {
            var hasDefaultConstructor = type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
            if (!hasDefaultConstructor)
            {
                throw new InvalidOperationException($"Type {type.FullName} must have a parameterless constructor.");
            }
        }
    }
}