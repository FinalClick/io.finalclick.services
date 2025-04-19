
# FinalClick - Services
**Unity Package: `com.finalclick.services`**

A lightweight and flexible service registration system for Unity, designed to simplify global and scene-specific service management through automatic discovery and structured lifecycles.

---

## Overview

`FinalClick.Services` allows you to register and resolve services both at the application level and per scene. The system is designed to integrate naturally with Unity's Play Mode lifecycle and scene loading events.

Services can be registered using static methods, allowing you to create pure csharp services, or via MonoBehaviours which lets you leverage references to assets or scene objects.


---

## Features

- 🔍 **Automatic Service Discovery** using the `RegisterServices` and `RegisterAsService` attributes.
- 💡 **Global Application Services** via `ApplicationServices`.
- 🎬 **Scene-scoped Services** via `SceneServices`.
- 🔗 **Inspector Unity Object Reference Support** when using MonoBehaviour services in scenes or application services prefab.
- ⚙️ **Lifecycle Hooks** via the `IService` interface.
- 🚀 **Disable Domain Reload Support**: automatic start and stops services during scene load and unload, and when entering or exiting Play Mode.

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

Or, using a Application Services Prefab, use the `[RegisterServices]` or `[RegisterAsService]` on a root MonoBehavior on the prefab:

1. Assign a application services prefab in the `Project Settings/ServiceSettings`.
2. Register services either:
   1. Define `[RegisterServices]` methods on components on the prefab, then manually register using ServiceCollectionBuilder functions.
   2. Define `[RegisterServiceAs(Type[])]` on a MonoBehaviour class, attach to serivces prefab, and it will be automatically registered.

This prefab will be automatically injected into the first scene that's loaded.

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

## Scene Service Registration

Scene services can be registered via MonoBehaviours.

#### For Scene Scope Services

1. Any `[RegisterServices]` methods on a root GameObjects MonoBehaviour will be called on scene load. 
2. Any `[RegisterAsService]` MonoBehaviours will be registered on scene load, IF on a root GameObject in the scene.

---

### Accessing Scene Services

You can retrieve services registered to a specific scene using:

via `MonoBehaviour` and `GameObject` extension methods. These automatically check the scene scope serivces for their scene before getting the service from the ApplicationServices.

```csharp
public class ExampleComponent : MonoBehavour
{
    void Awake()
    {
        this.GetService<IMyService>();
        gameobject.GetService<IMyService>();
    }
}
```

or via the Static class if working outside of unity code.

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
