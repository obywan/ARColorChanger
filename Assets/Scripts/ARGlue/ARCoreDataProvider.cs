using GoogleARCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ARCoreDataProvider : MonoBehaviour, IARDataProvider
{

    /// <summary>
    /// The first-person camera being used to render the passthrough camera.
    /// </summary>
    public Camera m_firstPersonCamera;

    private UnityEvent onSurfaceDetected;
    private bool weNeedWatchForPlaneDetection = true;
    private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

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

    private void OnEnable()
    {
        //FindObjectOfType<SessionComponent>().Connect();
    }

    private void Update()
    {
        if (weNeedWatchForPlaneDetection)
        {
            Session.GetTrackables(m_AllPlanes);
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    if(OnSurfaceDetected != null)
                        OnSurfaceDetected.Invoke();
                    weNeedWatchForPlaneDetection = false;
                    //m_AllPlanes.Clear();
                    break;
                }
            }
        }
    }

    public bool GetWorldPointFromTouchPosition(Vector2 touchPosition, out Vector3 worldPosition)
	{
		TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touchPosition.x, touchPosition.y, raycastFilter, out hit))
        {
            // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
            // world evolves.
            var anchor = hit.Trackable.CreateAnchor(hit.Pose);
            
            worldPosition = anchor.transform.position;
            return true;
        }

        worldPosition = Vector3.zero;
        return false;
	}
}
