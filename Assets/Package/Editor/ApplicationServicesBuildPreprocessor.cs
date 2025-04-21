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
            if (ServicesProjectSettings.TryGetServicesPrefab(out GameObject servicesInstance) == false)
            {
                servicesInstance = new GameObject("ApplicationServices");
                if (servicesInstance.scene != scene)
                {
                    throw new BuildFailedException("Services marker created in wrong scene.");
                }
            }

            SetGameObjectAsApplicationServices(servicesInstance);

            servicesInstance.AddComponent<ApplicationServicesMarker>();
            Debug.Log($"Injecting {servicesInstance.name} into {scene.name}");
        }
        
        private static void SetGameObjectAsApplicationServices(GameObject gameObject)
        {
            gameObject.AddComponent<ApplicationServicesMarker>();
        }
    }
}