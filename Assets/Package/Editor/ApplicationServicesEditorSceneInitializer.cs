using UnityEngine;

namespace FinalClick.Services.Editor
{
    public static class ApplicationServicesEditorSceneInitializer
    {
        public static void SetGameObjectAsApplicationServices(GameObject gameObject)
        {
            gameObject.AddComponent<ApplicationServicesMarker>();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnBeforeFirstSceneReady()
        {
            if (TryCreateApplicationServicesGameObjectFromPrefab(out var instance) == false)
            {
                instance = new GameObject("ApplicationServices");
            }

            SetGameObjectAsApplicationServices(instance.gameObject);
        }


        private static bool TryCreateApplicationServicesGameObjectFromPrefab(out GameObject instance)
        {
            if (ServicesProjectSettings.TryGetServicesPrefab(out GameObject prefab) == true)
            {
                instance = Object.Instantiate(prefab);
                return true;
            }

            instance = null;
            return false;
        }
    }
}