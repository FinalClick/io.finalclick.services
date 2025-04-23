using System;
using System.Reflection;
using FinalClick.Services.Attributes;
using UnityEngine;

namespace FinalClick.Services
{
    [Serializable]
    public class ApplicationServiceRegistrationData
    {
        [SerializeField] private string _serviceTypeName;

        private Type _cachedServiceType = null;

        public object CreateNewInstance()
        {
            return Activator.CreateInstance(GetServiceType());
        }
        
        public Type GetServiceType()
        {
            if (_cachedServiceType == null)
            {
                _cachedServiceType = Type.GetType(_serviceTypeName);
            }
            
            return _cachedServiceType;
        }

        public Type[] GetRegisterAsTypes()
        {
            var type = GetServiceType();
            
            Debug.Assert(type != null, "Type is null");
            
            RegisterAsApplicationServiceAttribute attribute = type.GetCustomAttribute(typeof(RegisterAsApplicationServiceAttribute), false) as RegisterAsApplicationServiceAttribute;
            
            Debug.Assert(attribute != null, "Type does not have attribute");

            if (attribute.RegisterSelfAsServiceType == true)
            {
                return new []{ type };
            }
            
            return attribute.RegisterTypes;
        }
        
        public bool IsDataStillValid()
        {
            var type = GetServiceType();
            
            // if type no longer exists, it's not valid.
            if (type == null)
            {
                return false;
            }
            
            // ensure the type still has the attribute
            return type.GetCustomAttributes(typeof(RegisterAsApplicationServiceAttribute), false).Length > 0;
        }

        public ApplicationServiceRegistrationData(Type type)
        {
            _serviceTypeName = type.AssemblyQualifiedName;
        }
    }
}