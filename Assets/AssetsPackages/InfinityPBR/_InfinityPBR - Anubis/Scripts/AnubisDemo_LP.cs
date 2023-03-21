using System.Collections;
using System.Collections.Generic;
using InfinityPBR;
using UnityEngine;
using UnityEngine.UI;

public class AnubisDemo_LP : MonoBehaviour
{
    public Renderer[] _renderer;
    public Renderer[] rendererGear;
    public BlendShapesManager[] bsmanager;
    public GameObject canvas;
    private Animator animator;

    public Toggle[] toggleWardrobe;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public void Locomotion(float newValue){
        animator.SetFloat ("locomotion", newValue);
    }

    public void ToggleCanvas()
    {
        canvas.SetActive(!canvas.activeSelf);
    }
    
    public void SetHue(float value)
    {
        for (int i = 0; i < _renderer.Length; i++)
        {
            _renderer[i].material.SetFloat("_Hue", value);
        }
    }
    
    public void SetSaturation(float value)
    {
        for (int i = 0; i < _renderer.Length; i++)
        {
            _renderer[i].material.SetFloat("_Saturation", value);
        }
    }
    
    public void SetValue(float value)
    {
        for (int i = 0; i < _renderer.Length; i++)
        {
            _renderer[i].material.SetFloat("_Value", value);
        }
    }
    
    public void SetHueGear(float value)
    {
        for (int i = 0; i < rendererGear.Length; i++)
        {
            rendererGear[i].material.SetFloat("_Hue", value);
        }
    }
    
    public void SetSaturationGear(float value)
    {
        for (int i = 0; i < rendererGear.Length; i++)
        {
            rendererGear[i].material.SetFloat("_Saturation", value);
        }
    }
    
    public void SetValueGear(float value)
    {
        for (int i = 0; i < rendererGear.Length; i++)
        {
            rendererGear[i].material.SetFloat("_Value", value);
        }
    }

    public void Randomize()
    {
        SetHue(Random.Range(0f,1f));
        SetSaturation(Random.Range(-.4f,.0f));
        SetValue(Random.Range(-.2f,.2f));
        
        SetHueGear(Random.Range(0f,1f));
        SetSaturationGear(Random.Range(-.2f,.2f));
        SetValueGear(Random.Range(-.2f,.2f));

        for (int i = 0; i < toggleWardrobe.Length; i++)
        {
            if (Random.Range(0, 2) == 1)
            {
                toggleWardrobe[i].isOn = true;
                toggleWardrobe[i].onValueChanged.Invoke(true);
            }
            else
            {
                toggleWardrobe[i].isOn = false;
                toggleWardrobe[i].onValueChanged.Invoke(false);
            }

        }
    }
}
