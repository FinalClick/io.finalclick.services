using System.Collections.Generic;
using UnityEngine;

namespace FinalClick.Services.Editor
{
    public class ServicesProjectSettingsConfigObject : ScriptableObject
    {
        [SerializeField]
        private GameObject _servicesPrefab;

        [SerializeField]
        private List<ApplicationServiceRegistrationSavedData> _applicationServiceData = new List<ApplicationServiceRegistrationSavedData>();
        
        public GameObject ServicesPrefab
        {
            get => _servicesPrefab;
            set => _servicesPrefab = value;
        }

        public List<ApplicationServiceRegistrationSavedData> ApplicationServiceData => _applicationServiceData;
    }
}