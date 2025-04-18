using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;

namespace FinalClick.Services
{
    public static class ApplicationServices
    {
        private static ServiceCollection _serviceCollection;
        private static bool IsStarted => _serviceCollection != null && _serviceCollection.IsStarted;
        
        internal static void StartFromGameObject(GameObject gameObject)
        {
            Debug.Assert(IsStarted == false, "Services already started");
            
            ServicesCollectionBuilder builder = new();

            builder.CallAllAutoRegisterStaticFunctions();
            builder.CallAllAutoRegisterFunctionsOnGameObject(gameObject);
            
            _serviceCollection = builder.Build();
            StartServices();
        }

        internal static bool TryUpdate()
        {
            if (IsStarted == false)
            {
                return false;
            }

            UpdateServices();
            return true;
        }

        private static void Stop()
        {
            if (IsStarted == false)
            {
                return;
            }
            
            UnbindDelegates();

            StopServices();
            _serviceCollection = null;
        }

        [UsedImplicitly]
        public static bool TryGet<TI>(out TI service)
        {
            return _serviceCollection.TryGet(out service);
        }

        [UsedImplicitly]
        public static TI Get<TI>()
        {
            return _serviceCollection.Get<TI>();
        }
        
        private static void StartServices()
        {
            Debug.Assert(IsStarted == false, "Services already started");
            
            Debug.Log("Starting application services...");
            
            ApplicationServicesUpdater.EnsureHasUpdater();
            
            _serviceCollection.StartServices();
            
            Debug.Log("Started application services.");
        }
        
        private static void UpdateServices()
        {
            Debug.Assert(IsStarted == true, "Services not started");
            _serviceCollection.UpdateServices();
            SceneServices.UpdateServices();
        }
        
        private static void StopServices()
        {
            Debug.Assert(IsStarted == true, "Services not started");
            Debug.Log("Stopping application services...");

            SceneServices.StopSceneServices();
            _serviceCollection.StopServices();
            
            Debug.Log("Stopped application services.");
        }

        // Ensure stop is called when exiting playmode or closing the application.
        // -----------------------------------------------------------------------
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void EnsureApplicationServicesUnregistersOnExit()
        {
            BindDelegates();
        }


#if UNITY_EDITOR
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Stop();
            }
        }
#endif
        
        private static void BindDelegates()
        {
            Application.quitting += Stop;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }
        
        private static void UnbindDelegates()
        {
            Application.quitting -= Stop;

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }

    }
}
