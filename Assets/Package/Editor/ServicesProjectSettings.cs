using UnityEditorInternal;
using UnityEngine;

namespace FinalClick.Services.Editor
{
    public static class ServicesProjectSettings
    {
        private const string ProjectSettingsPath = "ProjectSettings/FinalClickServiceSettings.asset";

        private static ServicesProjectSettingsConfigObject _loadedConfig = null;
        
        public static bool TryGetServicesPrefab(out GameObject servicesPrefab)
        {
            var configObject = GetOrCreateServicesSettingsConfigObject();

            if (configObject == null)
            {
                servicesPrefab = null;
                return false;
            }

            servicesPrefab = configObject.ServicesPrefab;
            return servicesPrefab != null;
        }

        private static ServicesProjectSettingsConfigObject GetOrCreateServicesSettingsConfigObject()
        {
            if (_loadedConfig == null)
            {
                var loadedObjects = InternalEditorUtility
                    .LoadSerializedFileAndForget(ProjectSettingsPath);
                if (loadedObjects.Length > 0)
                {
                    Debug.Assert(loadedObjects.Length == 1, "Too many objects were loaded.");
                    Debug.Assert(loadedObjects[0] is ServicesProjectSettingsConfigObject, $"Services saved object is not a {nameof(ServicesProjectSettingsConfigObject)}");
                    _loadedConfig = loadedObjects[0] as ServicesProjectSettingsConfigObject;
                }
            }

            if (_loadedConfig == null)
            {
                _loadedConfig = ScriptableObject.CreateInstance<ServicesProjectSettingsConfigObject>();   
                Debug.Log("Creating new Services Settings asset.");
                SaveConfigObject(_loadedConfig);
            }
            
            return _loadedConfig;
        }

        public static void SetServicesPrefab(GameObject servicesPrefab)
        {
            ServicesProjectSettingsConfigObject configObject = GetOrCreateServicesSettingsConfigObject();
            configObject.ServicesPrefab = servicesPrefab;
            Debug.Log("Updated Services Settings asset.");
            SaveConfigObject(configObject);
        }

        private static void SaveConfigObject(ServicesProjectSettingsConfigObject configObject)
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { configObject }, ProjectSettingsPath, true);
        }
    }
}