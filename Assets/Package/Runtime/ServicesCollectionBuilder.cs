using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FinalClick.Services.Attributes;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FinalClick.Services
{
    public class ServicesCollectionBuilder
    {
        private readonly Dictionary<Type, object> _registeredServices = new();
        private readonly List<IService> _managedServices = new();
        
        public ServiceCollection Build()
        {
            return new ServiceCollection(_managedServices, _registeredServices);
        }
        
        public void RunStaticRegisterFunctions()
        {
            var methods = RegisterServicesAttribute.GetAllStaticRegisterServicesMethods();
            InvokeRegisterMethods(methods);
        }

        public void RegisterGameObject(GameObject gameObject)
        {
            IService[] services = gameObject.GetComponentsInChildren<IService>(true);

            foreach (IService service in services)
            {
                RegisterManaged(service);
            }
            
            MonoBehaviour[] allComponents = gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in allComponents)
            {
                RunRegisterFunctionsOnMonoBehaviour(component);
            }
            
            RegisterAnyRegisterAsServiceMonoBehaviours(allComponents);
        }


        private void InvokeRegisterMethods(IEnumerable<MethodInfo> methods, object instance = null)
        {
            foreach (MethodInfo method in methods)
            {
                InvokeRegisterMethod(method, instance);
            }
        }
        
        private void InvokeRegisterMethod(MethodInfo method, object instance = null)
        {
            if (RegisterServicesAttribute.IsMethodValid(method) == false)
            {
                Debug.LogError($"Method {method.DeclaringType!.FullName}.{method.Name} marked with [{nameof(RegisterServicesAttribute)}] does not have the correct signature. Expected: static void Method(ServicesCollectionBuilder builder)");
            }

            method.Invoke(instance, new object[] { this });
        }

        private void RunRegisterFunctionsOnMonoBehaviour(MonoBehaviour monoBehaviour)
        {
            var methods = RegisterAsServiceAttribute.GetAllAutoRegisterServicesInstanceMethods(monoBehaviour);
            InvokeRegisterMethods(methods, monoBehaviour);
        }
        
        private void RegisterAnyRegisterAsServiceMonoBehaviours(MonoBehaviour[] allComponents)
        {
            foreach (var component in allComponents)
            {
                TryAutoRegisterAsService(component);
            }
        }
        
        private bool TryAutoRegisterAsService(MonoBehaviour monoBehaviour)
        {
            // Can be null if "The references scripted on this Behaviour (Unknown) is missing!" warnings.
            if (monoBehaviour == null)
            {
                return false;
            }
            
            RegisterAsServiceAttribute attribute = monoBehaviour.GetType().GetCustomAttribute(typeof(RegisterAsServiceAttribute), false) as RegisterAsServiceAttribute;

            if (attribute == null)
            {
                return false;
            }

            if (attribute.RegisterSelfAsServiceType == true)
            {
                Register(monoBehaviour, monoBehaviour.GetType());
            }
            else
            {
                Register(monoBehaviour, attribute.RegisterTypes);
            }
            return true;
        }

 
        [UsedImplicitly]
        public void Register(object service, params Type[] types)
        {
            foreach (var t in types)
            {

                if (t.IsInstanceOfType(service) == false)
                    throw new ArgumentException($"Service must be assignable to {t}", nameof(types));

                _registeredServices[t] = service;
            }

            if (service is IService managedService)
            {
                RegisterManaged(managedService);
            }
        }

        private void RegisterManaged(IService service)
        {
            if (_managedServices.Contains(service) == false)
            {
                _managedServices.Add(service);
            }
        }
        
        [UsedImplicitly]
        public void Register<TI, T>() where T : TI, new()
        {
            Register<TI, T>(new T());
        }

        [UsedImplicitly]
        public void Register<TI, T>(T service) where T : TI
        {
            Register(service, typeof(TI), typeof(T));
        }
        
        [UsedImplicitly]
        public void Register<T>(T service)
        {
            Register(service, typeof(T));
        }

        public void RegisterSceneServices(Scene scene)
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                RegisterGameObject(go);
            }
        }

        public void RegisterAutoRegisterAsApplicationAttributeService()
        {
            var types = RegisterAsApplicationServiceAttribute.GetTypesWithApplicationServiceAttribute();

            foreach (var type in types)
            {
                object instance;
                try
                {
                    instance = Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create instance of type {type.FullName}: {ex.Message}");
                    continue;
                }

                var attribute = type.GetCustomAttribute<RegisterAsApplicationServiceAttribute>(false);
                if (attribute == null)
                {
                    Debug.LogError($"Type {type.FullName} was expected to have {nameof(RegisterAsApplicationServiceAttribute)} but none was found.");
                    continue;
                }

                if (attribute.RegisterSelfAsServiceType == true)
                {
                    Register(instance, instance.GetType());
                }
                else
                {
                    
                    Register(instance, attribute.RegisterTypes);
                }
            }
        }
    }
}
