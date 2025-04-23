using System.Collections.Generic;
using System.Linq;
using FinalClick.Services.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace FinalClick.Services
{
    internal class RegisterApplicationServices : MonoBehaviour
    {
        [SerializeField] private ApplicationServiceRegistrationData[] _serviceData;

        [RegisterServices]
        [UsedImplicitly]
        private void RegisterNoneMonoBehaviourServices(ServicesCollectionBuilder builder)
        {
            foreach (var serviceData in _serviceData)
            {
                builder.Register(serviceData.CreateNewInstance(), serviceData.GetRegisterAsTypes());
            }
        }

        public void SetData(IReadOnlyCollection<ApplicationServiceRegistrationData> serviceRegisterData)
        {
            _serviceData = serviceRegisterData.ToArray();
        }
    }
}