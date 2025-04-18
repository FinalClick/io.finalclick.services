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
                    ServicesObjectField();
                }
            };

            return provider;
        }

        private static GameObject ServicesObjectField()
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