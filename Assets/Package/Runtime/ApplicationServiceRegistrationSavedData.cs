using System;
using FinalClick.Services.Attributes;
using UnityEngine;

namespace FinalClick.Services
{
    [Serializable]
    public class ApplicationServiceRegistrationSavedData
    {
        [SerializeField] private string _serviceTypeName;
        [SerializeField] private string _serviceAsJson;

        private Type _cachedServiceType = null;
        public string JsonData => _serviceAsJson;

        
        public void UpdateJson(object service)
        {
            _serviceAsJson = JsonUtility.ToJson(service);
        }
        
        public void OverwriteServiceFromData(object service)
        {
            JsonUtility.FromJsonOverwrite(_serviceAsJson, service);
        }
        
        public object CreateServiceFromData()
        {
            return JsonUtility.FromJson(_serviceAsJson, GetServiceType());
        }
        
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
            if (_cachedServiceType == null)
            {
                _cachedServiceType = Type.GetType(_serviceTypeName);
            }
            
            return _cachedServiceType;
        }

        public ApplicationServiceRegistrationSavedData(Type type)
        {
            var instance = Activator.CreateInstance(type);
            _serviceTypeName = type.AssemblyQualifiedName;
            UpdateJson(instance);
        }

    }
}