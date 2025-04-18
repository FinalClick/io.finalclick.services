
# FinalClick - Services
**Unity Package: `com.finalclick.services`**

A lightweight and flexible service registration system for Unity, designed to simplify global and scene-specific service management through automatic discovery and structured lifecycles.

---

## Overview

`FinalClick.Services` allows you to register and resolve services both at the application level and per scene. The system is designed to integrate naturally with Unity's Play Mode lifecycle and scene loading events.

Services can be registered using static methods, allowing you to create pure csharp services, or via MonoBehaviours which lets you leverage references to assets or scene objects.


---

## Features

- 🔍 **Automatic Service Discovery** using the `RegisterServices` attribute.
- 💡 **Global Application Services** via `ApplicationServices`.
- 🎬 **Scene-scoped Services** via `SceneServices`.
- 🔗 **MonoBehaviour-based Service Registration** for Unity object references.
- ⚙️ **Lifecycle Hooks** via the `IService` interface.
- 🚀 **Disable Domain Reload Support**: automatic start and stop during scene load and unload, and when entering or exiting Play Mode.

---

## Application Services

### Registering Services

To register services at the application level, create a static method marked with the `[RegisterServices]` attribute.

Example:

```csharp
[RegisterServices]
public static void RegisterMyServices(ServiceCollectionBuilder builder)
{
    builder.Register<IMyService, MyService>();
}
```

- The method must be `static`.
- It must accept exactly **one parameter**: `ServiceCollectionBuilder`.
- These methods are automatically called **before the first scene is loaded**.

---

### Accessing Application Services

Once registered, you can resolve services using:

```csharp
var service = ApplicationServices.Get<IMyService>();
```

or safely check:

```csharp
if (ApplicationServices.TryGet<IMyService>(out var service))
{
    service.DoSomething();
}
```

---

## MonoBehaviour-based Service Registration

In addition to static methods, services can also be registered via MonoBehaviours.

#### For Application Scope Services

1. Assign a application services prefab in the `Project Settings/ServiceSettings`.
2. Define `[RegisterServices]` methods on components on the prefab

This prefab will be added to the first loaded scene and automatically call the register functions before the default Awake order. This allows you to use the services collection builder to create application services before the first scene is fully loaded.

#### For Scene Scope Services

1. Attach a `SceneServicesObject` to a **root** GameObject
2. Any `[RegisterServices]` will be called on scene load and allow you to use the services collection builder to create scene services for the current scene.

*(Note: a separate attribute like `RegisterSceneServices` is planned for future versions.)*

---

### Accessing Scene Services

You can retrieve services registered to a specific scene using:

```csharp
var service = SceneServices.Get<IMySceneService>(scene);
```

or safely:

```csharp
if (SceneServices.TryGet<IMySceneService>(scene, out var service))
{
    service.DoSomething();
}
```

This allows services to be uniquely scoped to a scene instance, enabling scene-specific data, references, and logic separation.

---

## Service Lifecycle (`IService`)

For services that need structured startup, update, and shutdown behavior, implement the `IService` interface:

```csharp
namespace FinalClick.Services
{
    public interface IService
    {
        void OnServiceStart();
        void OnServiceUpdate();
        void OnServiceStop();
    }
}
```

| Method             | Application Service Called When | Scene Service Called When |
|---------------------|-------------------|---------------------------|
| `OnServiceStart()`  | On Application startup | On Scene Loaded           |
| `OnServiceUpdate()` | Once per frame    | Once per frame            |
| `OnServiceStop()`   | On Application shutdown | On Scene Unloaded         |

---

## Lifecycle Overview

| Event                             | Action                                             |
|-----------------------------------|---------------------------------------------------|
| Entering Play Mode                | Application services are registered and started.  |
| Loading a Scene                   | Scene services are registered and started.        |
| Unloading a Scene                 | Scene services are stopped.                       |
| Exiting Play Mode / Application Quit | Application and scene services are stopped.      |

---

## Roadmap

- [ ] Add a dedicated `RegisterSceneServices` attribute for clearer scene service separation.

---

## Summary

`com.finalclick.services` provides a clean, reliable way to structure both global and scene-level services in Unity. Whether you prefer pure C# or need asset/scene references through MonoBehaviours, this package helps you build robust systems with minimal boilerplate.
