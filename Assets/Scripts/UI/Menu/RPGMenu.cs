using LostRunes.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = LostRunes.Menu.AudioSettings;

namespace LostRunes.Menu
{
    public class RPGMenu : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] OptionMenuUI _optionMenu;
        public OptionMenuUI OptionMenuUI { get { return _optionMenu; } }
        [SerializeField] CustomMenuPanel _optionPanel;

        [Header("Audio Components")]
        [SerializeField] BackgroundMusicPlayer _backgroundMusicPlayer;
        [SerializeField] MenuNavigationSound _menuNavigationSound;

        protected CustomMenuPanel _basePanel;
        [SerializeField] protected CustomMenuPanel _activePanel;
        [SerializeField] protected CustomMenuPanel _previousPanel;

        public virtual void Initialize()
        {
            SubscribeAudioSources();
            LoadOptionData();
            PlayBackgroundMusic();
        }

        public void SetOptionPanelActive(bool active)
        {
            if (active)
            {
                _optionMenu.DisplayAudioOptionPanel();
            }
            else
            {
                _optionMenu.SaveSettings();
            }
            SetPanelActive(_optionPanel, active);
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
        private void LoadOptionData()
        {
            _optionMenu.LoadOptionData();
        }
        protected void SetPanelActive(CustomMenuPanel panel, bool active)
        {
            if (active)
            {
                _previousPanel = _activePanel;
                _previousPanel?.gameObject.SetActive(false);
                _activePanel = panel;
            }
            else
            {
                _activePanel = (_activePanel == _previousPanel) ? _basePanel : _previousPanel;
                _activePanel?.gameObject.SetActive(true);
            }

            panel?.gameObject.SetActive(active);
            _activePanel?.SelectDefaultSelectable();
        }
    }      
}