using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{

    public Action KeyAction = null;
    public Action<Define.TouchEvent> TouchAction = null; //대리 

    bool _pressed = false;
    float _pressedTime = 0;

    public void OnUpdate()//Touch의 종류를 매개변수로 전달함
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

            if (TouchAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    TouchAction.Invoke(Define.TouchEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                TouchAction.Invoke(Define.TouchEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                {
                    if (Time.time < _pressedTime + 0.2f)
                        TouchAction.Invoke(Define.TouchEvent.Click);
                    TouchAction.Invoke(Define.TouchEvent.PointerUp);
                }
                _pressed = false;
                _pressedTime = 0;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        TouchAction = null;
    }
}
