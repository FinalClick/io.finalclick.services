using System.Collections.Generic;
using UnityEngine;

namespace FinalClick.Services.Editor
{
    public class ServicesProjectSettingsConfigObject : ScriptableObject
    {
        [SerializeField]
        private GameObject _servicesPrefab;

        [SerializeField]
        private List<ApplicationServiceRegistrationData> _applicationServiceData = new List<ApplicationServiceRegistrationData>();
        
        public GameObject ServicesPrefab
        {
            get => _servicesPrefab;
            set => _servicesPrefab = value;
        }

        public List<ApplicationServiceRegistrationData> ApplicationServiceData => _applicationServiceData;
    }
}