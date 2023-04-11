using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOffseter : MonoBehaviour
{
    [SerializeField] Vector3 _offset;
    [SerializeField] float _distance;
    [SerializeField] bool _hideObjects;
    [ContextMenu("Start Offset")]
    public void StartOffset()
    {
        int i = 0;
        foreach(Transform child in transform)
        {
            child.transform.position = transform.position + _distance * (float)i * _offset;
            i++;
        }
    }

    [ContextMenu("Reset Offset")]
    public void ResetObjectPosition()
    {
        foreach (Transform child in transform)
        {
            child.transform.position = transform.position;
        }
    }

    [ContextMenu("Set Object Actives")]
    public void SetObjectActive()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(_hideObjects);
        }
    }
}
