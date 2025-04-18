using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace FinalClick.Services
{
    public static class SceneServices
    {
        private static readonly Dictionary<Scene, ServiceCollection> _sceneServices = new Dictionary<Scene, ServiceCollection>();

        
        [UsedImplicitly]
        public static bool TryGet<TI>(Scene scene, out TI service)
        {
            Debug.Assert(scene.IsValid() == true, "Scene is not valid");

            if (_sceneServices.TryGetValue(scene, out var services) == false)
            {
                throw new ArgumentException("Scene is not loaded", nameof(scene));
            }
            
            return services.TryGet(out service);
        }

        [UsedImplicitly]
        public static TI Get<TI>(Scene scene)
        {
            Debug.Assert(scene.IsValid() == true, "Scene is not valid");

            if (_sceneServices.TryGetValue(scene, out var services) == false)
            {
                throw new ArgumentException("Scene is not loaded", nameof(scene));
            }

            return services.Get<TI>();
        }
        
        private static void StartServicesForScene(Scene scene)
        { 
            Debug.Assert(_sceneServices.ContainsKey(scene) == false, "Services already started");
            
            var servicesObjects = GetSceneServiceObjects(scene);

            ServicesCollectionBuilder builder = new();

            foreach (SceneServicesObject servicesObject in servicesObjects)
            {
                builder.CallAllAutoRegisterFunctionsOnGameObject(servicesObject.gameObject);
            }
            
            var services = builder.Build();
            _sceneServices.Add(scene, services);
            services.StartServices();
            Debug.Log($"Started services for scene: {scene.name}({scene.handle})");
        }

        internal static void UpdateServices()
        {
            foreach (var sceneServices in _sceneServices)
            {
                sceneServices.Value.UpdateServices();
            }
        }

        internal static void StopSceneServices()
        {
            // Create a copy as StopServicesForScene will modify _sceneServices.
            var scenes = _sceneServices.Keys.ToList();

            foreach (var scene in scenes)
            {
                StopServicesForScene(scene);
            }
        }
        
        private static void StopServicesForScene(Scene scene)
        {
            if (_sceneServices.TryGetValue(scene, out var services) == false)
            {
                return;
            }
            
            services.StopServices();
            _sceneServices.Remove(scene);
            Debug.Log($"Stopped services for scene: {scene.name}({scene.handle})");
        }

        private static IReadOnlyList<SceneServicesObject> GetSceneServiceObjects(Scene scene)
        {
            List<SceneServicesObject> servicesObjects = new();
            foreach (var go in scene.GetRootGameObjects())
            {
                servicesObjects.AddRange(go.GetComponents<SceneServicesObject>());
            }
            
            return servicesObjects;
        }


        // Ensure stop is called when exiting playmode or closing the application.
        // -----------------------------------------------------------------------
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void EnsureApplicationServicesUnregistersOnExit()
        {
            BindDelegates();
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            StopServicesForScene(scene);
        }


        private static void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            StartServicesForScene(scene);
        }

        private static void BindDelegates()
        {
            Application.quitting += OnApplicationQuitting;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }
        
        private static void UnbindDelegates()
        {
            Application.quitting -= OnApplicationQuitting;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }

        private static void OnApplicationQuitting()
        {
            StopSceneServices();
            UnbindDelegates();
        }
        
#if UNITY_EDITOR
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                StopSceneServices();
                UnbindDelegates();
            }
        }
#endif

    }
}
