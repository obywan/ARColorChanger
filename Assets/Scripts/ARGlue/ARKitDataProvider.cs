using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_IOS
using UnityEngine.XR.iOS;
#endif

public class ARKitDataProvider : MonoBehaviour, IARDataProvider
{
#if UNITY_IOS
    private ARHitTestResultType[] resultTypes = {
                                        ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                                        // if you want to use infinite planes use this:
                                        //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                                        ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
                                        ARHitTestResultType.ARHitTestResultTypeFeaturePoint
                                    };
    private Vector3 result = Vector3.zero;
    private UnityEvent onSurfaceDetected;
    private Vector3[] aRCameraData;
    private bool weNeedWatchForPlaneDetection = true;

    public UnityEvent OnSurfaceDetected
    {
        get
        {
            return onSurfaceDetected;
        }

        set
        {
            onSurfaceDetected = value;
        }
    }

    public bool GetWorldPointFromTouchPosition(Vector2 touchposition, out Vector3 worldPosition)
    {
		Vector2 viewPortPoint = Camera.main.ScreenToViewportPoint (touchposition);
        ARPoint point = new ARPoint
        {
			x = viewPortPoint.x,
			y = viewPortPoint.y
        };

        foreach (ARHitTestResultType resultType in resultTypes)
        {
            if (HitTestWithResultType(point, resultType))
            {
                worldPosition = result;
                return true;
            }
        }
        worldPosition = result;
        return false;
    }

    private void Start()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += WatchForPlaneDetection;
    }

    private void WatchForPlaneDetection(UnityARCamera cam)
    {
        if (weNeedWatchForPlaneDetection)
        {
            aRCameraData = cam.pointCloudData;
            if(aRCameraData != null && aRCameraData.Length > 0)
            {
                OnSurfaceDetected.Invoke();
                weNeedWatchForPlaneDetection = false;
                aRCameraData = null;
            }

        }
    }

    private bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var hitResult in hitResults)
            {
                Debug.Log("Got hit!");
                result = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                //m_HitTransform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                return true;
            }
        }
        return false;
    }
#else
    public UnityEvent OnSurfaceDetected
    {
        get
        {
            return null;
        }

        set
        {
        }
    }

    public bool GetWorldPointFromTouchPosition(Vector2 touchposition, out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;
        return false;
    }


#endif

}
