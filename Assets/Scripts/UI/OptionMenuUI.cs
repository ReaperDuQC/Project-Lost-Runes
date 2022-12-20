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
        [SerializeField] Image _optionPanel;

        [SerializeField] Image _graphicOptionPanel;
        public GraphicSettings GraphicSettings { get { return _graphicSettings; } }
        [SerializeField] GraphicSettings _graphicSettings;

        [SerializeField] Image _audioOptionPanel;
        public AudioSettings AudioSettings { get { return _audioSettings; } }
        [SerializeField] AudioSettings _audioSettings;

        [SerializeField] Image _gameplayOptionPanel;
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