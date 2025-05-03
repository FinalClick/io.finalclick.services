using UnityEngine;

namespace FinalClick.Services.Editor
{
    internal static class ApplicationServicesEditorInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            ApplicationServicesBootstrapFactory.Create();
        }
    }
}