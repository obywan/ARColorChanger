using UnityEngine;
using UnityEngine.Events;

public class ARDataProvider : MonoBehaviour
{
    public bool arInEditor = false;
    public GameObject editorCamera;
    public GameObject arCoreStuffParent;
    public GameObject arKitStuffParent;

    public UnityEvent onSurfaceDetected;

    private IARDataProvider currentSource;

    private void Awake()
    {
#if UNITY_EDITOR
        if (arKitStuffParent)
            arKitStuffParent.SetActive(false);
        if (arCoreStuffParent)
            arCoreStuffParent.SetActive(arInEditor);
        editorCamera.SetActive(!arInEditor);

#elif UNITY_ANDROID
        if (arCoreStuffParent)
            arCoreStuffParent.SetActive(true);
        if(arKitStuffParent)
            arKitStuffParent.SetActive(false);

        currentSource = arCoreStuffParent.GetComponent<IARDataProvider>();
#elif UNITY_IOS
        if(arKitStuffParent)
            arKitStuffParent.SetActive(true);
        if(arCoreStuffParent)
            arCoreStuffParent.SetActive(false);
        currentSource = arKitStuffParent.GetComponent<IARDataProvider>();
#endif
    }

    private void OnEnable()
    {
        if(currentSource != null)
            currentSource.OnSurfaceDetected = onSurfaceDetected;
    }

    private void OnDisable()
    {
        if (currentSource != null)
            currentSource.OnSurfaceDetected.RemoveAllListeners();
    }

    public bool GetWorldPointByTouch (Vector2 touchPosition, out Vector3 wPos)
    {
        return currentSource.GetWorldPointFromTouchPosition(touchPosition, out wPos);
    }
}
