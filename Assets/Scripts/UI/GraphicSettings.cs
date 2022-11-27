using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Menu
{
    [System.Serializable]
    public class GraphicSettingsData
    {
        public int _framerate = 60;
        public int _screenResolutionX = 1920;
        public int _screenResolutionY = 1080;
        public int _screenRatioX = 4;
        public int _screenRatioY = 3;
        public bool _windowed;
        public GraphicSettingsData()
        {

        }
        public GraphicSettingsData(GraphicSettings graphicSettings)
        {
            _framerate = graphicSettings.Framerate;
            _screenResolutionX = graphicSettings.ScreenResolutionX;
            _screenResolutionY = graphicSettings.ScreenResolutionY;
            _screenRatioX = graphicSettings.ScreenRatioX;
            _screenRatioY = graphicSettings.ScreenRatioY;
            _windowed = graphicSettings.Windowed;
        }
    }
    public class GraphicSettings : MonoBehaviour
    {
        [SerializeField] int _framerate = 60;
        public int Framerate { get { return _framerate; } }
        [SerializeField] int _screenResolutionX = 1920;
        public int ScreenResolutionX { get { return _screenResolutionX; } }
       [SerializeField] int _screenResolutionY = 1080;
        public int ScreenResolutionY { get { return _screenResolutionY; } }
       [SerializeField] int _screenRatioX = 4;
        public int ScreenRatioX { get { return _screenRatioX; } }
       [SerializeField] int _screenRatioY = 3;
        public int ScreenRatioY { get { return _screenRatioY; } }
        [SerializeField] bool _windowed;
        public bool Windowed { get { return _windowed; } }  
        public void LoadGraphicSettingsData()
        {
            GraphicSettingsData data = SaveSystem.SaveSystem.LoadGraphicSettings();

            _framerate = data._framerate;
            _screenResolutionX = data._screenResolutionX;
            _screenResolutionY = data._screenResolutionY;
            _screenRatioX= data._screenRatioX;
            _screenRatioY = data._screenRatioY;
            _windowed = data._windowed;

            ApplySettings();
        }
        public void SaveGraphicSettings()
        {
            SaveSystem.SaveSystem.SaveGraphicSettings(this);
        }
        public void ResetToDefault()
        {
            _framerate = 60;
            _screenResolutionX = 1920;
            _screenResolutionY = 1080;
            _screenRatioX = 4;
            _screenRatioY = 3;
            _windowed = false;
        }
        public void UpdateFramerate(Int32 value)
        {
            _framerate = value;
        }
        void ApplySettings()
        {
            Screen.SetResolution(_screenResolutionX, _screenResolutionY, _windowed, _framerate);
        }
    }
}