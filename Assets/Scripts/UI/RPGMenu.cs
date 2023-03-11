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

        protected GameObject _basePanel;
        [SerializeField] protected GameObject _activePanel;
        [SerializeField] protected GameObject _previousPanel;

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
            SetPanelActive(_optionMenu.gameObject, active);
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
        protected void SetPanelActive(GameObject panel, bool active)
        {
            if (panel == null) { return; }

            if (active)
            {
                _previousPanel = _activePanel;
                _previousPanel.SetActive(false);
                _activePanel = panel;
            }
            else
            {
                _activePanel = (_activePanel == _previousPanel) ? _basePanel : _previousPanel;
                _activePanel.SetActive(true);
            }

            panel.SetActive(active);
        }
    }      
}