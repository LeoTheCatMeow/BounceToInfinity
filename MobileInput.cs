using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInput : MonoBehaviour
{
    public RectTransform inputFrame;
    public RectTransform virtualJoystick;
    public float maxJoystickDragRadius;

    static GameObject instance;
    public static Vector2 JoystickInput { get; private set; }
    public static bool buttonPressed { get; private set; }
    public static float screenDragDelta { get; private set; }

    private Vector2 initialPosition;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        } else
        {
            instance = gameObject;
        }
    }

    void OnDestroy()
    {
        if (instance == gameObject)
        {
            instance = null;
        }   
    }

    void Start()
    {
        initialPosition = virtualJoystick.anchoredPosition;
    }

    public void OnDragVirtualJoystick(BaseEventData data)
    {
        Vector2 dragVector = ScreenPointToAnchoredPosition(((PointerEventData)data).position) - initialPosition;
        dragVector = dragVector.normalized * Mathf.Min(dragVector.magnitude, maxJoystickDragRadius);
        JoystickInput = dragVector / new Vector2(maxJoystickDragRadius, maxJoystickDragRadius);
        virtualJoystick.anchoredPosition = initialPosition + dragVector;
    }

    public void EndDragVirtualJoystick(BaseEventData data)
    {
        virtualJoystick.anchoredPosition = initialPosition;
        JoystickInput = Vector2.zero;
    }

    public void OnButtonPress(BaseEventData data)
    {
        buttonPressed = true;
    }

    public void OnButtonRelease(BaseEventData data)
    {
        buttonPressed = false;
    }

    public void OnDragScreen(BaseEventData data)
    {
        screenDragDelta = ((PointerEventData)data).delta.x / Time.deltaTime / Screen.width;
    }

    public void EndDragScreen(BaseEventData data)
    {
        screenDragDelta = 0f;
    }

    private Vector2 ScreenPointToAnchoredPosition(Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inputFrame, screenPoint, null, out Vector2 x);
        return x;
    }

    
}
