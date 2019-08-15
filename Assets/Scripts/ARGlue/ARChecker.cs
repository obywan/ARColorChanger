using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ARChecker : MonoBehaviour
{
    public bool available = false;

    public UnityEvent arAvailable;
    public UnityEvent arNOTAvailable;
    
    void Start()
    {
        DoCheck();
    }

    public void DoCheck()
    {
#if UNITY_ANDROID
        CheckARCore();
#elif UNITY_IOS
        CheckARKit();
#endif
    }

    private void CheckARKit()
    {
        if (IsSupportedArKit())
        {
            available = true;
            if (arAvailable != null)
                arAvailable.Invoke();
        }
        else
        {
            available = false;
            if (arNOTAvailable != null)
                arNOTAvailable.Invoke();
        }
    }

    private void CheckARCore()
    {
        //#if UNITY_ANDROID
#if UNITY_ANDROID && !UNITY_EDITOR
        GoogleARCore.Session.CheckApkAvailability().ThenAction((result) =>
        {
            if (result == GoogleARCore.ApkAvailabilityStatus.SupportedInstalled)
            {
                available = true;
                if(arAvailable != null)
                    arAvailable.Invoke();
                return;
            }
            if (result == GoogleARCore.ApkAvailabilityStatus.SupportedNotInstalled)
            {
                available = true;
                if(arAvailable != null)
                    arAvailable.Invoke();
                return;
            }
            else
            if (result == GoogleARCore.ApkAvailabilityStatus.SupportedApkTooOld)
            {
                available = true;
                if(arAvailable != null)
                    arAvailable.Invoke();
                return;
            }
            else
            {
                available = false;
                if(arNOTAvailable != null)
                    arNOTAvailable.Invoke();
                return;
            }
        });
#endif
#if UNITY_EDITOR
        if (arNOTAvailable != null)
            arNOTAvailable.Invoke();
#endif
    }

    public bool IsSupportedArKit()
    {
#if UNITY_EDITOR
        return true;
#endif

#if UNITY_IOS
        Debug.Log("UnityEngine.iOS.Device.systemVersion:" + UnityEngine.iOS.Device.systemVersion);
        float iOSVersion = 11f;
        string[] ver = UnityEngine.iOS.Device.systemVersion.Split('.');
        float.TryParse(ver[0], out iOSVersion);
        if (iOSVersion < 11f)
        {
            Debug.Log("Not supported iOS version: " + iOSVersion);
            return false;
        }

        var gen = UnityEngine.iOS.Device.generation;
        Debug.Log("gen:" + gen);

        if (gen == UnityEngine.iOS.DeviceGeneration.iPhone4 ||
            gen == UnityEngine.iOS.DeviceGeneration.iPhone4S ||
            gen == UnityEngine.iOS.DeviceGeneration.iPhone5 ||
            gen == UnityEngine.iOS.DeviceGeneration.iPhone5C ||
            gen == UnityEngine.iOS.DeviceGeneration.iPhone5S ||
            gen == UnityEngine.iOS.DeviceGeneration.iPhone6 ||
            gen == UnityEngine.iOS.DeviceGeneration.iPhone6Plus ||
            gen == UnityEngine.iOS.DeviceGeneration.iPad1Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPad2Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPad3Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPad4Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPadAir1 ||
            gen == UnityEngine.iOS.DeviceGeneration.iPadAir2 ||
            gen == UnityEngine.iOS.DeviceGeneration.iPadMini1Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPadMini2Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPadMini3Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPadMini4Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen ||
            gen == UnityEngine.iOS.DeviceGeneration.iPodTouch6Gen)
        {
            Debug.Log("Device not supported");
            return false;
        }

        return true;
#endif

        return false;
    }
}
