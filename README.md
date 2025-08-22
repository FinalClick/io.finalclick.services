Installation: `"io.finalclick.services": "https://github.com/FinalClick/io.finalclick.services.git?path=/Assets/Package",`

# FinalClick – Services
**Unity Package:** `io.finalclick.services`

A simple lightweight **service locator** and **dependency injection** system for Unity.  
Use it to replace hard-referenced singletons with decoupled, testable services.

### Example
Instead of this:
```csharp
GameManager gameManager = GameManager.Instance;
```

Write this:
```csharp
GameManager gameManager = ApplicationServices.Get<GameManager>();
```

or this:
```csharp
GameManager gameManager = gameObject.GetService<GameManager>();
```
---

## Overview

`FinalClick.Services` allows you to register and resolve services both at the Project/Application level or at an Individual Scene level. Services can be pure C# classes or MonoBehaviours if you require references to assets or scene objects.

---

## Features

- 🔍 **Automatic Service Discovery** via `RegisterServices` and `RegisterAsService` attributes.
- 🧩 ***Automatic Dependency Injection** via the `InjectService` property attribute.
- 💡 **Project Level Services** via `ApplicationServices`.
- 🎬 **Scene-scoped Services** via `SceneServices`.
- ⚙️ **Lifecycle Hooks** via the `IService` interface. (`OnServiceStart`, `OnServiceUpdate`, `OnServiceStop`)
- 🚀 **Disable Domain Reload Support**: automatic start and stops services during scene load and unload, and when entering or exiting Play Mode.

> *Automatic Dependency Injection can only be used within service
---

## 🔍 Getting Services

### Inside a Service

```
[InjectService] 
            │
            └─ SceneServices.Get<T>(scene)
                │
                └─ ApplicationServices.Get<T>()
```

Within a Service you can use the `[InjectService]` attribute on a property

```csharp
[RegisterAsService]
public class ExampleComponent : MonoBehavour
{
    [InjectService] 
    private IMyService MyService { get; } = null;
    
    // ...
}
```

The service will be injected into the backing field of the property.

> Note, the `InjectService` attribute can only be used if the **MonoBehaviour** or csharp **class** is a registered service.

### Outside a Service

> Note, there are corresponding `TryGet` functions for the following `Get` functions

There are 3 ways to get services if the requester is not a service.

```
GameObject.GetService<T>()
                │
                └─ SceneServices.Get<T>(scene)
                    │
                    └─ ApplicationServices.Get<T>()
```

### GameObject Level

```csharp
ITimeService timeService = gameObject.GetService<ITimeService>;
```

This method requires access to a GameObject. It will first check if the scene the GameObject is in has the service it needs. If not found, it will then see if the Application has the service. This means you can have different service instances for each scene.

### Scene Level

```csharp
ITimeService timeService = SceneServices.Get<ITimeService>(scene);
```

This will require a reference to a scene, usually via `gameObject.scene`. It will first check if the scene has the service it needs. If not, it will check the ApplicationServices

### Application/Project Level

```csharp
ITimeService timeService = ApplicationServices.Get<ITimeService>(scene);
```

This can be called anywhere and requires no references to any Unity Object

## ⚙️ Application Service Registration

### Registering Pure C# Services

To register services at the project/application level create a static method marked with the `[RegisterServices]` attribute.

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

### Registering Unity MonoBehaviour Services

> Using MonoBehaviours allows you to reference Assets/Unity Objects easily via the inspector. 

Create a Prefab with your services. The services should be on the **root** GameObject of the prefab.
```csharp
[RegisterAsService]
public class MyService : MonoBehavour
{
    ///
}
```
or
```csharp
public class ServiceRegister : MonoBehavour
{
    [RegisterServices]
    private void MyRegisterFunction(ServiceCollectionBuilder builder)
    {
        builder.Register<OtherService>(OtherService);
    }
}
```
Assign the prefab in the **Project Settings** under `FinalClick > Services`

- Any `[RegisterServices]` methods on components on the prefab will be called with the application service collection builder.
- Any `[RegisterServiceAs(Type[])]` components on the prefab will be automatically registered.

This prefab will be automatically instantiated into the first scene that's loaded.

> Note, at build time this prefab is instantiated into the first scene. This causes it to be unpacked and baked at build time so there is no instantiation costs in builds.

---

## ⚙️ Scene Service Registration

Scene services can be registered via MonoBehaviours. However, the MonoBehaviour services must be on a **root** GameObject in the scene.

1. Any `[RegisterServices]` methods on a **root** GameObjects MonoBehaviour will be called on scene load. 
2. Any `[RegisterAsService]` MonoBehaviours, which are also on **roo** GameObjects, will be registered on scene load

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
