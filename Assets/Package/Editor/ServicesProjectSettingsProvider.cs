using System;
using FinalClick.Services.Attributes;
using UnityEditor;
using UnityEngine;

namespace FinalClick.Services.Editor
{
    public class ServicesProjectSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateServicesSettingsProvider()
        {
            var provider = new SettingsProvider("Project/FinalClick/Services", SettingsScope.Project)
            {
                label = "Services",
                guiHandler = (_) =>
                {
                    ServicesPrefabObjectField();
                    EditorGUILayout.Space();
                    ServicesDataField();
                }
            };

            return provider;
        }

        private static void ServicesDataField()
        {
            var applicationServiceDataList = ServicesProjectSettings.GetApplicationServiceData();

            if (applicationServiceDataList == null || applicationServiceDataList.Count == 0)
            {
                EditorGUILayout.LabelField("No Application Services registered.");
                return;
            }

            foreach (var savedData in applicationServiceDataList)
            {
                var serviceType = savedData.GetServiceType();
                if (serviceType == null)
                {
                    EditorGUILayout.LabelField("Missing Service", "(Type Not Found)");
                    continue;
                }

                var attribute = (RegisterAsApplicationServiceAttribute)Attribute.GetCustomAttribute(serviceType, typeof(RegisterAsApplicationServiceAttribute));

                if (attribute == null)
                {
                    EditorGUILayout.LabelField(serviceType.FullName, "(Missing RegisterAsApplicationServiceAttribute)");
                    continue;
                }

                EditorGUILayout.LabelField($"Service: {serviceType.FullName}");

                if (attribute.RegisterSelfAsServiceType)
                {
                    EditorGUILayout.LabelField("  Registered As:", serviceType.FullName);
                }
                else
                {
                    EditorGUILayout.LabelField("  Registered As:");
                    foreach (var regType in attribute.RegisterTypes)
                    {
                        EditorGUILayout.LabelField($"    - {regType.FullName}");
                    }
                }
            }
        }

        private static GameObject ServicesPrefabObjectField()
        {
            ServicesProjectSettings.TryGetServicesPrefab(out GameObject savedServicesPrefab);
            
            GameObject selectedServicesPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Application Services",
                savedServicesPrefab,
                typeof(GameObject),
                false);

            // If the selected prefab changes, update the saved settings
            if (savedServicesPrefab != selectedServicesPrefab)
            {
                ServicesProjectSettings.SetServicesPrefab(selectedServicesPrefab);
            }

            return selectedServicesPrefab;
        }
    }
}