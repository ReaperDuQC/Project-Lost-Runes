using RPG.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSettings = RPG.Menu.AudioSettings;

namespace RPG
{
    public class RPGMenu : MonoBehaviour
    {
        [SerializeField] AudioSource _menuNavigationSource;
        [SerializeField] AudioClip _menuNavigationMusic;
        public AudioSettings AudioSettings { get { return _audioSettings; } }
        [SerializeField] AudioSettings _audioSettings;
        public void PlayNavigationMenuSound()
        {
            if (_menuNavigationSource == null) return;
            if (_menuNavigationMusic == null) return;
            AudioSettings.RandomizePitch(_menuNavigationSource);
            if (_menuNavigationSource.clip == null)
            {
                _menuNavigationSource.clip = _menuNavigationMusic;
            }
            _menuNavigationSource.Play();
        }
        virtual protected void SubscribeAudioSources()
        {
            _audioSettings.SubscribeToSfxAudioSource(_menuNavigationSource);
        }
    }
}