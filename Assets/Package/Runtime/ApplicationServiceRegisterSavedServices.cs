using System.Collections.Generic;
using System.Linq;
using FinalClick.Services.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace FinalClick.Services
{
    internal class RegisterSavedApplicationServices : MonoBehaviour
    {
        [SerializeField] private ApplicationServiceRegistrationSavedData[] _serviceData;

        [RegisterServices]
        [UsedImplicitly]
        private void RegisterServicesFromSaved(ServicesCollectionBuilder builder)
        {
            foreach (var serviceData in _serviceData)
            {
                var type = serviceData.GetServiceType();

                if (builder.TryGet(type, out object service) == false)
                {
                    Debug.LogError($"Unable to find service {type} to override with saved data.");
                    continue;
                }
                
                Debug.Log($"Overwriting service: {type}");
                serviceData.OverwriteServiceFromData(service);
            }
        }

        public void SetData(IReadOnlyList<ApplicationServiceRegistrationSavedData> servicesData)
        {
            _serviceData = servicesData.ToArray();
        }
    }
}