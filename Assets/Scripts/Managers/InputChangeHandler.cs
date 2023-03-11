using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputChangeHandler : MonoBehaviour
{
    public UnityEvent _controllerUsed;
    public UnityEvent _keyboardUsed;


    private void Awake()
    {
        InputUser.onChange += DeviceChanged;
    }

    private void DeviceChanged(InputUser user, InputUserChange change, InputDevice device)
    {
       Debug.Log(user.ToString() + " " + change.ToString() + " "+device.ToString());
    }
}
