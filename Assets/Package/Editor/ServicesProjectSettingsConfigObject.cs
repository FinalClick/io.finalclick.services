using UnityEngine;

namespace FinalClick.Services.Editor
{
    internal class ServicesProjectSettingsConfigObject : ScriptableObject
    {
        [SerializeField]
        private GameObject _servicesPrefab;

        public GameObject ServicesPrefab
        {
            get => _servicesPrefab;
            set => _servicesPrefab = value;
        }
    }
}