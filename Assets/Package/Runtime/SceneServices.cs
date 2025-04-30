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

        public static bool HasStartedForScene(Scene scene)
        {
            Debug.Assert(scene.IsValid() == true, "Scene is not valid");
            
            if (_sceneServices.TryGetValue(scene, out var services) == false)
            {
                return false;
            }

            return true;
        }
        
        [UsedImplicitly]
        public static bool TryGet<TI>(Scene scene, out TI service)
        {
            Debug.Assert(scene.IsValid() == true, "Scene is not valid");

            if (_sceneServices.TryGetValue(scene, out var services) == false)
            {
                service = default;
                return false;
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
            
            ServicesCollectionBuilder builder = new();

            builder.RegisterSceneServices(scene);
            
            var services = builder.Build();
            _sceneServices.Add(scene, services);
            
            // Create a gameobject in the scene created, and then add a scene stopper.
            GameObject stopped = new GameObject("SceneServiceStopper");
            stopped.transform.SetParent(scene.GetRootGameObjects()[0].transform);
            stopped.transform.parent = null;
            stopped.AddComponent<SceneServiceStopper>();
            stopped.transform.SetAsFirstSibling();
            
            services.StartServices(ApplicationServices.ServiceCollection);
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
        
        internal static void StopServicesForScene(Scene scene)
        {
            if (_sceneServices.TryGetValue(scene, out var services) == false)
            {
                return;
            }
            
            services.StopServices();
            _sceneServices.Remove(scene);
            Debug.Log($"Stopped services for scene: {scene.name}({scene.handle})");
        }

        private static IReadOnlyList<MonoBehaviour> GetRootMonoBehaviours(Scene scene)
        {
            List<MonoBehaviour> rootMonos = new();
            foreach (var go in scene.GetRootGameObjects())
            {
                rootMonos.AddRange(go.GetComponents<MonoBehaviour>());
            }
            
            return rootMonos;
        }


        // Ensure stop is called when exiting playmode or closing the application.
        // -----------------------------------------------------------------------
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void EnsureApplicationServicesUnregistersOnExit()
        {
            BindDelegates();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            StartServicesForScene(scene);
        }

        private static void BindDelegates()
        {
            Application.quitting += OnApplicationQuitting;
            SceneManager.sceneLoaded += OnSceneLoaded;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }
        
        private static void UnbindDelegates()
        {
            Application.quitting -= OnApplicationQuitting;
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
