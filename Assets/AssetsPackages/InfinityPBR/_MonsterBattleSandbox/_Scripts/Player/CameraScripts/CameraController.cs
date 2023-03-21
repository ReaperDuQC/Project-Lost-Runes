using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;
    [SerializeField] private CinemachineVirtualCamera vCam;

    [Range(10f, 20f)]
    public float orthographicSize = 11.3f;
    public bool mouseEdgeMove = false;

    private Vector2 previousInput;

    private Controls controls;

    void Start()
    {
        vCam.m_Lens.OrthographicSize = orthographicSize;
        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controls();

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;

        controls.Enable();
    }

    private void Update()
    {
        UpdateCameraPosition();
        UpdateCameraFOV();
    }

    private void UpdateCameraFOV()
    {
        if (Input.mouseScrollDelta.y < 0)
        {
            vCam.m_Lens.OrthographicSize += Time.deltaTime * 20;
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            vCam.m_Lens.OrthographicSize -= Time.deltaTime * 20;
        }
    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;

        if (previousInput == Vector2.zero && mouseEdgeMove == true)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if (cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = pos;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }
}
