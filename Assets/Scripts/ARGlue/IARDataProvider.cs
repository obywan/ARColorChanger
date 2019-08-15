using UnityEngine;
using UnityEngine.Events;

public interface IARDataProvider
{
    UnityEvent OnSurfaceDetected { get; set; }

    bool GetWorldPointFromTouchPosition(Vector2 touchposition, out Vector3 worldPosition);
}