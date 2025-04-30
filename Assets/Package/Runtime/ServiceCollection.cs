using System;
using System.Collections.Generic;
using System.Linq;
using FinalClick.Services.Injection;
using UnityEngine;

namespace FinalClick.Services
{
    public class ServiceCollection
    {
        public bool IsStarted => _isStarted;

        private readonly IReadOnlyList<IService> _managedServices;
        private readonly IReadOnlyDictionary<Type, object> _registeredServices;
        private bool _isStarted = false;

        public ServiceCollection CreateNewCombinedCollection(ServiceCollection other)
        {
            var registered = new Dictionary<Type, object>();
            foreach (var service in _registeredServices)
            {
                registered[service.Key] = service.Value;
            }
            foreach (var service in other._registeredServices)
            {
                registered[service.Key] = service.Value;
            }
            
            var managed = new List<IService>();
            managed.AddRange(_managedServices);
            managed.AddRange(other._managedServices);
            
            return new ServiceCollection(managed, registered);
        }
        
        public ServiceCollection(IReadOnlyList<IService> managedServices, IReadOnlyDictionary<Type, object> registeredServices)
        {
            _managedServices = managedServices.ToList();
            _registeredServices = new Dictionary<Type, object>(registeredServices);
        }

        public bool TryGet<TI>(out TI service)
        {
            if (TryGet(typeof(TI), out var serviceAsObject) == false)
            {
                service = default;
                return false;
            }

            if (serviceAsObject is TI typedService)
            {
                service = typedService;
                return true;
            }

            service = default;
            return false;
        }

        public bool TryGet(Type type, out object service)
        {
            return _registeredServices.TryGetValue(type, out service);
        }

        public object Get(Type type)
        {
            if (TryGet(type, out object service) == false)
            {
                throw new InvalidOperationException($"No service registered for type {type}");
            }

            return service;
        }

        public TI Get<TI>()
        {
            return (TI) Get(typeof(TI));
        }
        
        public void StartServices(ServiceCollection outerScopeServices = null)
        {
            InjectServices(outerScopeServices);
            
            Debug.Assert(_isStarted == false, "Services already started");

            _isStarted = true;
            
            foreach (IService service in _managedServices)
            {
                try
                {
                    service.OnServiceStart();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        
        private void InjectServices(ServiceCollection outerScopeServices = null)
        {
            Debug.Log("Injecting services...");

            var services =  _registeredServices.Values.Distinct();

            var injectables = this;

            if (outerScopeServices != null)
            {
                injectables = injectables.CreateNewCombinedCollection(outerScopeServices);
            }
            
            foreach (var service in services)
            {
                try
                {
                    ServiceInjection.Inject(injectables, service);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            Debug.Log("Injecting services. Completed..");
        }
        
        public void UpdateServices()
        {
            Debug.Assert(_isStarted == true, "Services not started");
            
            foreach (IService service in _managedServices)
            {
                try
                {
                    service.OnServiceUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        
        public void StopServices()
        {
            if (_isStarted == false)
            {
                return;
            }
            
            foreach (IService service in _managedServices)
            {
                try
                {
                    service.OnServiceStop();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
