using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSchemeTracker : MonoBehaviour
{
    public delegate void ControlSchemeChangedCallback(string controlSchemeName/* new control Scheme*/);
    public ControlSchemeChangedCallback _controlSchemeChanged;

    // class to keep track of active control scheme
    // should update UI accordingly
    // should update Debug control Scheme

    InputDevice _activeInputDevice;

    InputDevice lastDevice;

    private void Start()
    {
        //InputSystem.onActionChange += (obj, change) =>
        //{
        //    if (change == InputActionChange.ActionPerformed)
        //    {
        //        var inputAction = (InputAction)obj;
        //        var lastControl = inputAction.activeControl;
        //        var lastDevice = lastControl.device;

        //        Debug.Log($"device: {lastDevice.displayName}");
        //    }
        //};
    }
private void OnEnable()
    {
       // InputSystem.onDeviceChange += DeviceChangeCallback;
    }
    private void OnDisable()
    {
        //InputSystem.onDeviceChange -= DeviceChangeCallback;
    }

    private void DeviceChangeCallback(InputDevice newInputDevice, InputDeviceChange deviceChange)
    {

        if (_activeInputDevice != null)
        {
            if (_activeInputDevice == newInputDevice) return;
        }
        _activeInputDevice = newInputDevice;

        if (_controlSchemeChanged != null)
        {
            _controlSchemeChanged(newInputDevice.name);
        }
        
    }
}
