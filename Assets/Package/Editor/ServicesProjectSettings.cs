using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace FinalClick.Services.Editor
{
    public static class ServicesProjectSettings
    {
        private const string SettingsPath = "Assets/FinalClickServiceSettings.asset";

        private static ServicesProjectSettingsConfigObject _loadedConfig = null;
        
        public static bool TryGetServicesPrefab(out GameObject servicesPrefab)
        {
            var configObject = GetOrCreateServicesSettingsConfigObject();

            servicesPrefab = configObject.ServicesPrefab;
            return servicesPrefab != null;
        }
        
        public static IReadOnlyList<ApplicationServiceRegistrationData> GetApplicationServiceRegistrationData()
        {
            var configObject = GetOrCreateServicesSettingsConfigObject();
            return configObject.ApplicationServiceData;
        }
        
        private static ServicesProjectSettingsConfigObject GetOrCreateServicesSettingsConfigObject()
        {
            if (_loadedConfig == null)
            {
                var loadedObjects = InternalEditorUtility
                    .LoadSerializedFileAndForget(SettingsPath);
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
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { configObject }, SettingsPath, true);
        }

        public static List<ApplicationServiceRegistrationData> GetApplicationServiceData()
        {
            var configObject = GetOrCreateServicesSettingsConfigObject();
            return configObject.ApplicationServiceData;
        }

        private static void SyncApplicationServiceDataWithCurrentTypes()
        {
            var configObject = GetOrCreateServicesSettingsConfigObject();
            if (configObject == null) return;

            var validTypes = FinalClick.Services.Attributes.RegisterAsApplicationServiceAttribute.GetTypesWithApplicationServiceAttribute().ToHashSet();

            // Remove any types that should no longer be registered.
            configObject.ApplicationServiceData.RemoveAll(data => data.IsDataStillValid() == false);

            foreach (var type in validTypes)
            {
                bool exists = configObject.ApplicationServiceData.Exists(data => data.GetServiceType() == type);
                if (!exists)
                {
                    ApplicationServiceRegistrationData newData = new ApplicationServiceRegistrationData(type);
                    configObject.ApplicationServiceData.Add(newData);
                }
            }

            SaveConfigObject(configObject);
        }

        [InitializeOnLoadMethod]
        private static void OnProjectRecompile()
        {
            SyncApplicationServiceDataWithCurrentTypes();
        }

        public static void Save()
        {
            SaveConfigObject(GetOrCreateServicesSettingsConfigObject());
        }

        public static ServicesProjectSettingsConfigObject GetConfig()
        {
            return GetOrCreateServicesSettingsConfigObject();
        }
    }
}