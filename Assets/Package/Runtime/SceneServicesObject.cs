using UnityEngine;

namespace FinalClick.Services
{
    public class SceneServicesObject : MonoBehaviour
    {
        private void OnValidate()
        {
            if (IsInScene() == true)
            {
                Debug.Assert(IsRootGameObject() == true, $"SceneServicesObject must be a root GameObject of the Scene. '{this.name}' will not be found by the service locator.");
            }
        }

        private bool IsInScene()
        {
            return gameObject.scene.IsValid() && gameObject.scene.isLoaded;
        }

        private bool IsRootGameObject()
        {
            Debug.Assert(IsInScene() == true, "Can't check if GameObject is not in a scene. (e.g. prefab asset)");

            return gameObject.transform.parent == null;
        }
    }
}