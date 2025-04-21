using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using UnityEditor;

namespace FinalClick.Services.Editor
{
    public static class ServicesProjectSettings
    {
        private const string ProjectSettingsPath = "ProjectSettings/FinalClickServiceSettings.asset";

        private static ServicesProjectSettingsConfigObject _loadedConfig = null;
        
        public static bool TryGetServicesPrefab(out GameObject servicesPrefab)
        {
            var configObject = GetOrCreateServicesSettingsConfigObject();

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

        public static List<ApplicationServiceRegistrationSavedData> GetApplicationServiceData()
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
            configObject.ApplicationServiceData.RemoveAll(data => data.DoesServiceTypeStillRequireRegistration() == false);

            foreach (var type in validTypes)
            {
                bool exists = configObject.ApplicationServiceData.Exists(data => data.GetServiceType() == type);
                if (!exists)
                {
                    ApplicationServiceRegistrationSavedData newData = new ApplicationServiceRegistrationSavedData(type);
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
    }
}