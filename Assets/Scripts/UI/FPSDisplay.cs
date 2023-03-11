using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public delegate void FPSUpdated(int fps);
    public FPSUpdated _fpsUpdated;
    [SerializeField] float _pollingTime = 1f;
    float _time;
    int _frameCount;

    private void Update()
    {
        _time += Time.deltaTime;

        _frameCount++;

        if (_time >= _pollingTime)
        {
            int frameRate = Mathf.FloorToInt(_frameCount / _time);

            if(_fpsUpdated!= null)
            {
                _fpsUpdated(frameRate);
            }
            _time -= _pollingTime;
            _frameCount = 0;
        }
    }
}
