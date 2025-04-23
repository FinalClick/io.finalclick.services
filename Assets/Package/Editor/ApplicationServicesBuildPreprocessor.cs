using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FinalClick.Services.Editor
{
    public class ApplicationServicesBuildPreprocessor : IProcessSceneWithReport
    {
        public int callbackOrder => -1;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // Only inject into the first scene bool scene
            if (SceneManager.GetSceneAt(0) != scene)
            {
                return;
            }

            // If already created application services, we dont need to do again.
            if(Application.isPlaying == true && ApplicationServices.HasStarted() == true)
            {
                return;
            }
            
            // Create from prefab in settings, or make blank gameobject.
            bool usePrefab = ServicesProjectSettings.TryGetServicesPrefab(out GameObject servicesPrefab);

            GameObject servicesInstance = usePrefab ? GameObject.Instantiate(servicesPrefab) : new GameObject("Application Services");

            AddSavedServicesToApplicationServices(servicesInstance);
            SetGameObjectAsApplicationServices(servicesInstance);
            
            Debug.Log($"Injecting {servicesInstance.name} into {scene.name}");
        }

        private void AddSavedServicesToApplicationServices(GameObject gameObject)
        {
            var component = gameObject.AddComponent<RegisterApplicationServices>();
            var servicesData = ServicesProjectSettings.GetApplicationServiceRegistrationData();
            component.SetData(servicesData);
        }
        
        private static void SetGameObjectAsApplicationServices(GameObject gameObject)
        {
            gameObject.AddComponent<ApplicationServicesMarker>();
        }
    }
}