using JetBrains.Annotations;
using UnityEngine;

namespace FinalClick.Services
{
    public static class MonoBehaviourExtensions
    {
        [UsedImplicitly]
        public static bool TryGetService<T>(this MonoBehaviour monoBehaviour, out T service)
        {
            return monoBehaviour.gameObject.TryGetService<T>(out service);
        }

        [UsedImplicitly]
        public static T GetService<T>(this MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.gameObject.GetService<T>();
        }
    }
}