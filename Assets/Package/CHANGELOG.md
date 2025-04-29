# Changelog


## [1.0.7] - 2025-04-29

### Changed
- Scene Services are now unloaded by a GameObject added to each scene, rather than Unload callback from scene manager.
  - This is to fix an issue where the service that are gameobjects were already destroyed before the Unload callback.

## [1.0.6] - 2025-04-23

### Changed
- Ensure services are not initialized again if loading back into the first scene.

## [1.0.5] - 2025-04-23

### Added
- Added support to auto register pure csharp classes with the `RegisterAsService` attribute
  - They must have a default constructor

###
- General bug fixes

## [1.0.4] - 2025-04-19

### Changed
- Renamed ProjectSettings file from `ServicesSettings` to `FinalClickServiceSettings`

## [1.0.3] - 2025-04-19

### Added
- **GameObject** and **MonoBehaviour** extension methods `this.GetService<T>` and `this.TryGetService<T>`
  - These functions will first try to get the scene service for a type, and then fall back to the application service if the scene doesnt have that type.
- `RegisterAsService` attribute to automatically register MonoBehaviours as services.
  - SceneServices: The attribute must be on a MonoBehaviour on a root GameObject in the Scene.
  - ApplicationServices: The attribute must be on a MonoBehaviour on the root GameObject of the prefab. 
- 

## [1.0.2] - 2025-04-19

### Changed
- Fixed `ApplicationServicesUpdates` not being created
- Renamed `CallAllAutoRegisterStaticFunctions` to `RunStaticRegisterFunctions`
- Renamed `CallAllAutoRegisterFunctionsOnGameObject` to `RegisterGameObject`

### Added
- Added function `RegisterManaged` for `ServiceCollectionBuilder` so you can register a managed service without a type
- Added function `HasStartedForScene` for `SceneServices` to check if scenes services has been created and started
- Added function `HasStarted` for `ApplicationServices` to check if application services has been created and started 
- Added `ApplicationServicesTests` and `SceneServicesTests`

---

## [1.0.1] — 2025-04-18

### Changed
- Renamed `RegisterApplicationServices` to `RegisterGameObject`

---

## [1.0.0] — 2025-04-18

### Added
- Initial release of `com.finalclick.services` 🎉
- Application-wide service registration via static methods with `[RegisterApplicationServices]`.
- Scene-specific service registration via `SceneServicesObject` and `[RegisterApplicationServices]` on components.
- Support for MonoBehaviour-based service registration with asset references via prefab in Project Settings or asset/scene-object references with a SceneServiceObject in scenes.
- `ApplicationServices.Get<T>()` and `ApplicationServices.TryGet<T>()` for global service resolution.
- `SceneServices.Get<T>(Scene)` and `SceneServices.TryGet<T>(Scene, out T)` for scene-scoped service resolution.
- `IService` interface for structured lifecycle management:
    - `OnServiceStart()`
    - `OnServiceUpdate()`
    - `OnServiceStop()`
- Automatic service startup and teardown during Play Mode transitions and scene load/unload.

---