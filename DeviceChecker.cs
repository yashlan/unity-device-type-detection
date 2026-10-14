using System;
using UnityEngine;

public static class DeviceDetector
{
    public static bool IsTablet()
    {
        if (Application.isEditor) //Whether the game is running inside the Unity Editor
            return IsTabletByScreenSize();

#if UNITY_IOS

        return IsIPad();

#elif UNITY_ANDROID

        // 1. Native Android detection
        bool? nativeResult = IsTabletNative();

        if (nativeResult.HasValue)
            return nativeResult.Value;

        // 2. Fallback if native detection error/null
        return IsTabletByScreenSize();

#else

        // Editor / Windows / Mac / other platforms
        return IsTabletByScreenSize();

#endif
    }


#if UNITY_IOS

    private static bool IsIPad()
    {
        try
        {
            string device = UnityEngine.iOS.Device.generation.ToString();

            if (device.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                return true;

            if (device.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
                return false;

            return false;
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                $"iOS device detection failed: {e.Message}"
            );

            return false;
        }
    }

#endif


#if UNITY_ANDROID

    private static bool? IsTabletNative()
    {
        try
        {
            using (var unityPlayer =
                new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var resources =
                activity.Call<AndroidJavaObject>("getResources"))
            using (var configuration =
                resources.Call<AndroidJavaObject>("getConfiguration"))
            {
                // Android's smallestScreenWidthDp.
                // 600dp or more is commonly used as the
                // large-screen / tablet breakpoint.

                int smallestWidthDp =
                    configuration.Get<int>("smallestScreenWidthDp");

                // Get screen resolution
                int widthPx = Screen.width;
                int heightPx = Screen.height;

                // Calculate aspect ratio
                float aspectRatio =
                    Mathf.Max(widthPx, heightPx) /
                    Mathf.Min(widthPx, heightPx);

                // Very wide aspect ratios are typical of phones.
                // Examples:
                // 20:9  = 2.22
                // 19.5:9 = 2.17
                // 19:9  = 2.11
                //
                // If the device is clearly wide, treat it as a phone
                // even if the Android smallestScreenWidthDp is reported
                // incorrectly by the manufacturer.

                if (aspectRatio >= 1.65f)
                    return false;

                // For tablet-like aspect ratios:
                // 16:10 = 1.60
                // 16:9  = 1.78
                // 4:3   = 1.33
                //
                // Use Android's smallestScreenWidthDp as the
                // secondary tablet check.

                return smallestWidthDp >= 600;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                $"Native Android tablet detection failed: {e.Message}"
            );

            return null;
        }
    }

#endif

    private static bool IsTabletByScreenSize()
    {
        // Calculate aspect ratio first.
        float aspectRatio =
            Mathf.Max(Screen.width, Screen.height) /
            Mathf.Min(Screen.width, Screen.height);

        // Clearly wide screen → phone.
        // Examples:
        // 20:9  = 2.22
        // 19.5:9 = 2.17
        // 19:9  = 2.11
        if (aspectRatio >= 1.65f)
            return false;

        float dpi = Screen.dpi;

        // Screen.dpi can be invalid in Unity Editor / Device Simulator.
        if (dpi <= 0f)
        {
            int shortSide = Mathf.Min(
                Screen.width,
                Screen.height
            );

            // Fallback approximation.
            return shortSide >= 900;
        }

        float widthInches = Screen.width / dpi;
        float heightInches = Screen.height / dpi;

        float diagonalInches = Mathf.Sqrt(
            widthInches * widthInches +
            heightInches * heightInches
        );

        // Treat screens with a diagonal size of 7 inches or more as tablets.
        return diagonalInches >= 7.0f;
    }
}
