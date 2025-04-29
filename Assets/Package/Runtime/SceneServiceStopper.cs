using System;
using UnityEngine;

namespace FinalClick.Services
{
    public class SceneServiceStopper : MonoBehaviour
    {
        private void OnDestroy()
        {
            SceneServices.StopServicesForScene(gameObject.scene);
        }
    }
}