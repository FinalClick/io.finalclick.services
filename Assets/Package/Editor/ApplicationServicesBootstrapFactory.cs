using UnityEngine;

namespace FinalClick.Services.Editor
{
    public class ApplicationServicesBootstrapFactory
    {
        internal static GameObject Create()
        {
            // Create from prefab in settings, or make blank gameobject.
            bool usePrefab = ServicesProjectSettings.TryGetServicesPrefab(out GameObject servicesPrefab);

            GameObject servicesInstance = usePrefab ? GameObject.Instantiate(servicesPrefab) : new GameObject("Application Services");

            AddSavedServicesToApplicationServices(servicesInstance);
            SetGameObjectAsApplicationServices(servicesInstance);
            
            return servicesInstance;
        }

        private static void AddSavedServicesToApplicationServices(GameObject gameObject)
        {
            var component = gameObject.AddComponent<RegisterApplicationServices>();
            var servicesData = ServicesProjectSettings.GetApplicationServiceRegistrationData();
            component.SetData(servicesData);
        }
        
        private static void SetGameObjectAsApplicationServices(GameObject gameObject)
        {
            gameObject.AddComponent<ApplicationServicesMarker>();

        }
    }
}