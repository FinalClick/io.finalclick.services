using System;
using UnityEngine;

namespace FinalClick.Services
{
    [DefaultExecutionOrder(-1000)]
    public class SceneServiceStopper : MonoBehaviour
    {
        private void OnDestroy()
        {
            SceneServices.StopServicesForScene(gameObject.scene);
        }
    }
}