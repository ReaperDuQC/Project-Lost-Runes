using LostRunes.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = LostRunes.Menu.AudioSettings;

namespace LostRunes
{
    public class RPGMenu : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] OptionMenuUI _optionMenu;
        public OptionMenuUI OptionMenuUI { get { return _optionMenu; } }

        [Header("Audio Components")]
        [SerializeField] BackgroundMusicPlayer _backgroundMusicPlayer;
        [SerializeField] MenuNavigationSound _menuNavigationSound;

        // List of buttons in the scene to be linked with the sound making script for menu naviagation
        [Header("UI Buttons")]

        [SerializeField] List<Button> _returnButtons;
        [SerializeField] List<Button> _normalButtons;

        private void Awake()
        {
            //_returnButtons = null; 
            //_normalButtons = null;

            //Button[] allButtons = FindObjectsOfType<Button>();

            //foreach (var button in allButtons)
            //{
            //    if (button.gameObject.name.StartsWith("Return"))
            //    {
            //        _returnButtons.Add(button);
            //    }
            //    else
            //    {
            //        _normalButtons.Add(button);
            //    }
            //}
        }
        public virtual void Initialize()
        {
            SubscribeAudioSources();
            LinkButtons();
            LoadOptionData();
            PlayBackgroundMusic();
        }

        public void ToggleOptionPanel()
        {
            if (_optionMenu == null) { return; }

            _optionMenu.gameObject.SetActive(!_optionMenu.gameObject.activeSelf);
        }
        void PlayBackgroundMusic()
        {
            if (_backgroundMusicPlayer == null) return;

            _backgroundMusicPlayer.InitializeAudioSource();
        }
        void SubscribeAudioSources()
        {
            if (_backgroundMusicPlayer != null)
            {
                _backgroundMusicPlayer.SubscribeAudioSource(_optionMenu.AudioSettings);
            }

            if (_menuNavigationSound != null)
            {
                _menuNavigationSound.SubscribeAudioSource(_optionMenu.AudioSettings);
            }
        }
        void LinkButtons()
        {
            foreach (Button b in _normalButtons)
            {
                b.onClick.AddListener(_menuNavigationSound.PlayForwardNavigationSound);
            }
            foreach (Button b in _returnButtons)
            {
                b.onClick.AddListener(_menuNavigationSound.PlayBackwardNavigationSound);
            }
        }
        private void LoadOptionData()
        {
            _optionMenu.LoadOptionData();
        }
    }      
}