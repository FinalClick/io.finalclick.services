using UnityEngine;

namespace FinalClick.Services
{
    [DefaultExecutionOrder(-1000)]
    internal class ApplicationServicesMarker : MonoBehaviour
    {
        private void Awake()
        {
            if (ApplicationServices.HasStarted() == true)
            {
                DestroyImmediate(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
            ApplicationServices.StartFromGameObject(gameObject);
        }
    }
}