# Unity Device Type Detection

A lightweight C# utility to detect whether a device is a phone or tablet in Unity, supporting Android phones and tablets, iPhones, and iPads.

## Features

- Detect Android phones and tablets.
- Detect iPhones and iPads.
- Supports Unity Editor with screen-size-based detection.
- Uses native Android screen configuration when available.
- Provides fallback detection using screen size and aspect ratio.

## How to Use

**1. Add `DeviceDetector.cs` to your Unity project.**

**2. Call `DeviceDetector.IsTablet()` from any C# script.**

```csharp
using UnityEngine;

public class YourClassName : MonoBehaviour
{
    private void Start()
    {
        if (DeviceDetector.IsTablet())
        {
            Debug.Log("Device is a tablet.");
        }
        else
        {
            Debug.Log("Device is a phone.");
        }
    }
}
```

## Supported Platforms

- Android (mobile and tablet)
- iOS (iPhone and iPad)
- Unity Editor (screen-size-based detection)
- 
