using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollingText : MonoBehaviour
{
    public float duration = 1f;
    public float speed;

    private Camera cameraForStatEvents;
    private TextMeshPro textMesh;
    //private TextMesh textMesh;
    private float startTime;

    void Awake()
    {
        cameraForStatEvents = Camera.main;
        textMesh = GetComponent<TextMeshPro>();
        //textMesh = GetComponent<TextMesh>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime < duration)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        textMesh.SetText(text);
    }

    public void SetColor(Color color)
    {
        textMesh.color = color;
    }
}
