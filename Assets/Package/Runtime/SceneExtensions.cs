using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FinalClick.Services
{
    public static class SceneExtensions
    {
        [UsedImplicitly]
        public static bool TryGetService<T>(this Scene scene, out T service)
        {
            if (SceneServices.TryGet<T>(scene, out service) == true)
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
        public static T GetService<T>(this Scene scene)
        {
            if (TryGetService<T>(scene, out var service) == false)
            {
                throw new ArgumentException("Service not found.");
            }
            
            return service;
        }
    }
}