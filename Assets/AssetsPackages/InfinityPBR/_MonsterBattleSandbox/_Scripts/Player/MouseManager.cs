using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class EventVector3 : UnityEvent<Vector3> { }

[System.Serializable]
public class EventGameObject : UnityEvent<GameObject> { }

public class MouseManager : MonoBehaviour
{
    [SerializeField] private bool useDefaultCursor = false;
    public LayerMask clickableLayer;
    [SerializeField] Camera playerCamera;

    public Texture2D pointer;
    //public Texture2D target;
    //public Texture2D doorway;
    //public Texture2D sword;

    public EventVector3 OnClickEnvironment;
    public EventVector3 OnRightClickEnvironment;
    public EventGameObject OnClickAttackable;

    private void Start()
    {
        if (useDefaultCursor == false)
        {
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
        }
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, 75, clickableLayer.value))
        {
            bool isAttackable = hit.collider.GetComponent(typeof(IAttackable)) != null;

            // If environment surface is clicked, invoke callbacks.
            if (Input.GetMouseButtonDown(0))
            {

            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (isAttackable)
                {
                    GameObject attackable = hit.collider.gameObject;
                    OnClickAttackable.Invoke(attackable);
                }
                else
                {
                    OnRightClickEnvironment.Invoke(hit.point);
                }
            }
        }
    }
}