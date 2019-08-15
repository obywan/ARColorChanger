using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlacementController : MonoBehaviour {
    public enum ActionMode { NONE, MOVE, ROTATE };
    public const float TOUCH_COOLDOWN = 1f;

    public bool scalable = false;
    public ARDataProvider arDataProvider;
    public Transform m_targetObj;
	public UnityEvent eventsAfterFirstPlacing;

    private int touchPosition = 0;

    private Touch t0;
    private Touch t1;

    private Vector2 t0InitPos;
    private Vector2 t1InitPos;

    private ActionMode actionMode = ActionMode.NONE;

    private float countdouwnTimer = TOUCH_COOLDOWN;

    public bool placedFirstTime = false;

    // Use this for initialization
    private void Start()
    {
        //m_targetObj.position = new Vector3(0f, 1000f, 0f);
    }

    // Update is called once per frame
    private void Update()
    {
        WatchInput();
    }


    
    private void WatchInput ()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if ((countdouwnTimer <= 0f) || (!placedFirstTime))
                MoveItem(Input.mousePosition);
        }
#endif


        SetActionModeBasedOnTouches(Input.touches);

        switch (Input.touchCount)
        {
            case 1:
                // handle one touch
                HandleOneTouch();
                break;
            case 2:
                // handle two touches
                HandleTwotouches();
                break;
            default:
                //nothing to do
                break;
        }
    }

    private void HandleOneTouch ()
    {
        if (IsClickOnGUI())
            return;


        t0 = Input.GetTouch(0);

        if (t0.phase == TouchPhase.Began || t0.phase == TouchPhase.Moved || t0.phase == TouchPhase.Stationary)
        {
            countdouwnTimer -= Time.deltaTime;
            if ((countdouwnTimer <= 0f) || (!placedFirstTime))
                MoveItem(t0.position);
            //if (!placedFirstTime)

        }

        if (t0.phase == TouchPhase.Ended)
            countdouwnTimer = TOUCH_COOLDOWN;
    }

    private void HandleTwotouches ()
    {
        UpdateRotation();
    }

    private void MoveItem (Vector2 position)
    {
        if (actionMode == ActionMode.MOVE)
        {
            Vector3 destinationPosition;

            if (arDataProvider.GetWorldPointByTouch(position, out destinationPosition))
            {
                if (!m_targetObj.gameObject.activeSelf)
                    m_targetObj.gameObject.SetActive(true);
                m_targetObj.position = destinationPosition;

                if (!placedFirstTime)
                {

                    if (eventsAfterFirstPlacing != null)
                        eventsAfterFirstPlacing.Invoke();

                    m_targetObj.LookAt(Camera.main.transform);
                    Vector3 tmpRotation = m_targetObj.localEulerAngles;
                    m_targetObj.localEulerAngles = new Vector3(0f, tmpRotation.y, 0f);
                    placedFirstTime = true;
                }
            }
        }
    }

    private void UpdateRotation()
    {
        if (actionMode == ActionMode.ROTATE)
        {
            t0 = Input.GetTouch(0);
            t1 = Input.GetTouch(1);

            if (t1.phase == TouchPhase.Began)
            {
                t0InitPos = t0.position;
                t1InitPos = t1.position;
            }

            if (t0InitPos != null && t1InitPos != null)
            {
                if (t0.phase == TouchPhase.Moved)
                {
                    float angle = Vector2.Angle(Input.touches[0].position - t1InitPos, Quaternion.Euler(0, 0, 90) * (t0InitPos - t1InitPos));
                    angle -= 90;
                    m_targetObj.Rotate(Vector3.up, angle * 3f);
                }
                if (t1.phase == TouchPhase.Moved)
                {
                    float angle = Vector2.Angle(Input.touches[1].position - t0InitPos, Quaternion.Euler(0, 0, 90) * (t1InitPos - t0InitPos));
                    angle -= 90;
                    m_targetObj.Rotate(Vector3.up, angle * 3f);
                }

                //change scale if set so
                if(scalable)
                    m_targetObj.localScale *= Vector3.Distance(t0.position, t1.position) / Vector3.Distance(t0InitPos, t1InitPos);
            }

            t0InitPos = t0.position;
            t1InitPos = t1.position;
        }
    }

    private void SetActionModeBasedOnTouches (Touch[] touches)
    {
        if (touches.Length > 0)
        {
            switch (touches.Length)
            {
                case 1:
                    if (touches[0].phase == TouchPhase.Began || touches[0].phase == TouchPhase.Moved)
                        SetActionMode(ActionMode.MOVE);
                    if (touches[0].phase == TouchPhase.Ended)
                        SetActionMode(ActionMode.NONE);
                    break;
                case 2:
                    if (touches[1].phase == TouchPhase.Began)
                        SetActionMode(actionMode = ActionMode.ROTATE);
                    if (touches[1].phase == TouchPhase.Ended)
                        SetActionMode(ActionMode.NONE);
                    break;
                default:
                    break;
            }
        }
    }

    private void SetActionMode (ActionMode mode)
    {
        actionMode = mode;
    }

    private bool IsClickOnGUI()
    {
        if (EventSystem.current == null)
            return false;

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        if (Input.touches.Length > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId);
        else
            return false;
#else

        return EventSystem.current.IsPointerOverGameObject();
#endif
    }


    public void ResetPlacement()
    {
        placedFirstTime = false;
    }
}
