using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FinalClick.Services
{
    public class ServiceCollection
    {
        public bool IsStarted => _isStarted;
        
        private readonly IReadOnlyList<IService> _managedServices;
        private readonly IReadOnlyDictionary<Type, object> _registeredServices;
        private bool _isStarted = false;

        public ServiceCollection(IReadOnlyList<IService> managedServices, IReadOnlyDictionary<Type, object> registeredServices)
        {
            _managedServices = managedServices.ToList();
            _registeredServices = new Dictionary<Type, object>(registeredServices);
        }

        public bool TryGet<TI>(out TI service)
        {
            if (_registeredServices.TryGetValue(typeof(TI), out var instance) == true)
            {
                service = (TI)instance;
                return true;
            }

            service = default;
            return false;
        }

        public TI Get<TI>()
        {
            if (TryGet(out TI instance) == true)
            {
                return instance;
            }

            throw new InvalidOperationException($"No service registered for type {typeof(TI)}");
        }
        
        public void StartServices()
        {
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
