using LostRunes.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = LostRunes.Menu.AudioSettings;

namespace LostRunes
{
    public class OptionMenuUI : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] GameObject _optionPanel;

        [SerializeField] GameObject _graphicOptionPanel;
        public GraphicSettings GraphicSettings { get { return _graphicSettings; } }
        [SerializeField] GraphicSettings _graphicSettings;

        [SerializeField] GameObject _audioOptionPanel;
        public AudioSettings AudioSettings { get { return _audioSettings; } }
        [SerializeField] AudioSettings _audioSettings;

        [SerializeField] GameObject _gameplayOptionPanel;
        public GameplaySettings GameplaySettings { get { return _gameplaySettings; } }
        [SerializeField] GameplaySettings _gameplaySettings;

        public void DisplayVideoOptionPanel()
        {
            _graphicOptionPanel.gameObject.SetActive(true);
            _audioOptionPanel.gameObject.SetActive(false);
            _gameplayOptionPanel.gameObject.SetActive(false);
        }
        public void DisplayAudioOptionPanel()
        {
            _graphicOptionPanel.gameObject.SetActive(false);
            _audioOptionPanel.gameObject.SetActive(true);
            _gameplayOptionPanel.gameObject.SetActive(false);
        }
        public void DisplayGameplayOptionPanel()
        {
            _graphicOptionPanel.gameObject.SetActive(false);
            _audioOptionPanel.gameObject.SetActive(false);
            _gameplayOptionPanel.gameObject.SetActive(true);
        }
        public void LoadOptionData()
        {
            _audioSettings.LoadAudioSettingsData();
            _graphicSettings.LoadGraphicSettingsData();
            _gameplaySettings.LoadGameplaySettingsData();
        }
    }
}