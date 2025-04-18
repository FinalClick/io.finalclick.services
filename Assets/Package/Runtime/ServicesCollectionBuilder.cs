using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

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
        
        public void CallAllAutoRegisterStaticFunctions()
        {
            var methods = GetAllStaticRegisterApplicationServicesMethods();
            InvokeRegisterMethods(methods);
        }

        public void CallAllAutoRegisterFunctionsOnGameObject(GameObject gameObject)
        {
            MonoBehaviour[] allComponents = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (MonoBehaviour component in allComponents)
            {
                CallAllAutoRegisterFunctionsOnMonoBehaviour(component);
            }
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
            if (RegisterApplicationServicesAttribute.IsMethodValid(method) == false)
            {
                Debug.LogError($"Method {method.DeclaringType!.FullName}.{method.Name} marked with [{nameof(RegisterApplicationServicesAttribute)}] does not have the correct signature. Expected: static void Method(ServicesCollectionBuilder builder)");
            }

            method.Invoke(instance, new object[] { this });
        }

        private void CallAllAutoRegisterFunctionsOnMonoBehaviour(MonoBehaviour monoBehaviour)
        {
            var methods = GetAllAutoRegisterApplicationServicesInstanceMethods(monoBehaviour);
            InvokeRegisterMethods(methods, monoBehaviour);
        }

        private static IEnumerable<MethodInfo> GetAllAutoRegisterApplicationServicesInstanceMethods(MonoBehaviour monoBehaviour)
        {
            var methods = monoBehaviour.GetType()
                .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                .Where(method => method.GetCustomAttributes(typeof(RegisterApplicationServicesAttribute), false).Length > 0);
            
            return methods;
        }
        
        private static IEnumerable<MethodInfo> GetAllStaticRegisterApplicationServicesMethods()
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                .Where(method => method.GetCustomAttributes(typeof(RegisterApplicationServicesAttribute), false).Length > 0);
            
            return methods;
        }

        [UsedImplicitly]
        public void Register(object service, Type type, params Type[] types)
        {
            if (type.IsInstanceOfType(service) == false)
                throw new ArgumentException($"Service must be assignable to {type}", nameof(service));

            _registeredServices[type] = service;

            foreach (var t in types)
            {
                if (t.IsInstanceOfType(service) == false)
                    throw new ArgumentException($"Service must be assignable to {t}", nameof(types));

                _registeredServices[t] = service;
            }

            if (service is IService managedService && _managedServices.Contains(managedService) == false)
            {
                _managedServices.Add(managedService);
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
            Register(service, typeof(TI));
        }
    }
}
