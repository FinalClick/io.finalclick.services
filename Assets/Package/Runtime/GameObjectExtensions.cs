using System;
using JetBrains.Annotations;
using UnityEngine;

namespace FinalClick.Services
{
    public static class GameObjectExtensions
    {
        [UsedImplicitly]
        public static bool TryGetService<T>(this GameObject gameObject, out T service)
        {
            if (SceneServices.TryGet<T>(gameObject.scene, out service) == true)
            {
                return true;
            }

            // Fall back to application scope if scene doesnt have that service.
            if (ApplicationServices.TryGet<T>(out service) == true)
            {
                return true;
            }

            return false;
        }

        [UsedImplicitly]
        public static T GetService<T>(this GameObject gameObject)
        {
            if (TryGetService<T>(gameObject, out var service) == false)
            {
                throw new ArgumentException("Service not found.");
            }
            
            return service;
        }
    }
}