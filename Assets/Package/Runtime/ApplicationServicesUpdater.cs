using UnityEngine;

namespace FinalClick.Services
{
    // -1 to gaurantee services are updated before all other components by default.
    [DefaultExecutionOrder(-1)]
    public class ApplicationServicesUpdater : MonoBehaviour
    {
        private static ApplicationServicesUpdater _instance;
        
        private static bool HasUpdater => _instance != null;
        
        public static void EnsureHasUpdater()
        {
            if (HasUpdater == false)
            {
                return;
            }

            Debug.Assert(_instance == null, "ApplicationServiceUpdater already exists");
            
            GameObject go = new GameObject("ApplicationServicesUpdater");
            _instance = go.AddComponent<ApplicationServicesUpdater>();
            DontDestroyOnLoad(go);
        }

        private void Update()
        {
            ApplicationServices.TryUpdate();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}