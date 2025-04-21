using System;
using FinalClick.Services.Attributes;
using UnityEngine;

namespace FinalClick.Services
{
    [Serializable]
    public class ApplicationServiceRegistrationSavedData
    {
        [SerializeField] private string _serviceTypeName;

        public bool DoesServiceTypeStillRequireRegistration()
        {
            var type = GetServiceType();
            
            // if type no longer exists
            if (type == null)
            {
                return false;
            }
            
            return type.GetCustomAttributes(typeof(RegisterAsApplicationServiceAttribute), false).Length > 0;
        }
        
        public Type GetServiceType()
        {
            return Type.GetType(_serviceTypeName);
        }

        public ApplicationServiceRegistrationSavedData(Type type)
        {
            _serviceTypeName = type.AssemblyQualifiedName;
        }

    }
}