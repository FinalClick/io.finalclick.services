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
            // ApplicationServivcesEditorInitializer handles in editor, as it needs to be created before the awake of other objects.
            if(Application.isPlaying == true)
            {
                return;
            }
            
            // Only inject into the first scene boot scene, as then it will always be started first.
            if (SceneManager.GetSceneAt(0) != scene)
            {
                return;
            }

            var servicesInstance = ApplicationServicesBootstrapFactory.Create();
            Debug.Log($"Injecting {servicesInstance.name} into {scene.name}");
        }
    }
}