using UnityEngine;

namespace FinalClick.Services
{
    [DefaultExecutionOrder(-1000)]
    internal class ApplicationServicesMarker : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ApplicationServices.StartFromGameObject(gameObject);
        }
    }
}