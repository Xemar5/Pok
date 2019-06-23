using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchHandler : MonoBehaviour
#if UNITY_ANDROID
    , IPointerDownHandler, IPointerUpHandler
#endif
{
    [SerializeField]
    private string axisName = string.Empty;
    [Range(-1f, 1f)]
    [SerializeField]
    private float direction = 0;

    private bool isClicked = false;
    private bool lastFrameInputValue;

    public event Action<TouchHandler> OnTouchDown;
    public event Action<TouchHandler> OnTouchUp;

    public float Direction => direction;

#if UNITY_ANDROID
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isClicked == false)
        {
            OnDown();
            isClicked = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isClicked == true)
        {
            OnUp();
            isClicked = false;
        }
    }
#endif

#if UNITY_WEBGL
    private void Update()
    {
        float value = Input.GetAxis(axisName);
        bool inputValue = value > 0.5f;
        if (inputValue == true && lastFrameInputValue == false && isClicked == false)
        {
            OnDown();
            isClicked = true;
        }
        else if (inputValue == false && isClicked == true)
        {
            OnUp();
            isClicked = false;
        }
        lastFrameInputValue = inputValue;
    }

#endif

    private void OnDown()
    {
        OnTouchDown?.Invoke(this);
    }
    private void OnUp()
    {
        OnTouchUp?.Invoke(this);
    }


}
