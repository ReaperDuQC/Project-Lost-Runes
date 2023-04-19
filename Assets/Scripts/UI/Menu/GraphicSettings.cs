using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostRunes.Menu
{
    [System.Serializable]
    public class GraphicSettingsData
    {
        public int _targetFramerate = -1;
        public int _resolutionIndex = -1;

        public bool _fullscreen = false;
        public GraphicSettingsData()
        {

        }
        public GraphicSettingsData(GraphicSettings graphicSettings)
        {
            _targetFramerate = graphicSettings.TargetFramerate;

            _resolutionIndex = graphicSettings.CurrentResolutionIndex;

            _fullscreen = graphicSettings.Fullscreen;
        }
    }
    public class GraphicSettings : MonoBehaviour
    {
        [SerializeField] int _targetFramerate = -1;
        public int TargetFramerate { get { return _targetFramerate; } }

        [SerializeField] bool _fullscreen = false;
        public bool Fullscreen { get { return _fullscreen; } }

        [SerializeField] Toggle _fullscreenToggle;
        [SerializeField] TMP_Dropdown _resolutionDropDown;

        Resolution[] _resolutions;
        List<Resolution> _filteredResolutions;

        float _currentRefreshRate;
        int _currentResolutionIndex = -1;
        public int CurrentResolutionIndex { get { return _currentResolutionIndex; } }
        public void LoadGraphicSettingsData()
        {
            GraphicSettingsData data = SaveSystem.SaveSystem.LoadGraphicSettings();
            
            Initialize();

            if (data._resolutionIndex != -1)
            {
                _targetFramerate = data._targetFramerate;
                _currentResolutionIndex = data._resolutionIndex;
                _fullscreen = data._fullscreen;
            }

            ApplyToggleValue();
            SetResolution();
        }

        void ApplyToggleValue()
        {
            _fullscreenToggle.isOn = _fullscreen;
        }

        private void Initialize()
        {
            _targetFramerate = -1;
            _currentResolutionIndex = -1;
            _fullscreen = false;

            _resolutions = Screen.resolutions;
            _filteredResolutions = new();

            _resolutionDropDown.ClearOptions();
            _currentRefreshRate = Screen.currentResolution.refreshRate;

            for(int i = 0; i < _resolutions.Length; i++)
            {
                if (_resolutions[i].refreshRate == _currentRefreshRate)
                {
                    _filteredResolutions.Add(_resolutions[i]);
                }
            }

            List<string> options = new List<string>();

            for(int i = 0; i < _filteredResolutions.Count ; i++)
            {
                string resolutionOption = _filteredResolutions[i].width + "x" + _filteredResolutions[i].height + " " + _filteredResolutions[i].refreshRate + "Hz";
                options.Add(resolutionOption);
                if(_filteredResolutions[i].width == Screen.currentResolution.width && _filteredResolutions[i].height == Screen.currentResolution.height)
                {
                    _currentResolutionIndex = i;
                }
            }

            _resolutionDropDown.AddOptions(options);
            //_resolutionDropDown.itemText.autoSizeTextContainer = true;
            _resolutionDropDown.value = _currentResolutionIndex;
            _resolutionDropDown.RefreshShownValue();
        }
        public void ToggleFullscreen(bool fullscreen) 
        {
            _fullscreen = fullscreen;

            SetResolution();
        }
        public void SaveGraphicSettings()
        {
            SaveSystem.SaveSystem.SaveGraphicSettings(this);
        }
        public void ResetToDefault()
        {
            Initialize();
            ApplyToggleValue();
            SetResolution();
        }
        public void UpdateFramerate(Int32 value)
        {
            _targetFramerate = value;
        }

        public void SetResolutionIndex(int resolutionIndex)
        {
            _currentResolutionIndex = resolutionIndex;

            SetResolution();
        }

        private void SetResolution()
        {
            Resolution resolution = _filteredResolutions[_currentResolutionIndex == -1 ? 0: _currentResolutionIndex];

            Screen.SetResolution(resolution.width, resolution.height, _fullscreen);

            Application.targetFrameRate = _targetFramerate;
        }
    }
}