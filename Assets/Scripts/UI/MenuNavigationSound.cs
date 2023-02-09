using LostRunes.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSettings = LostRunes.Menu.AudioSettings;

namespace LostRunes
{
    public class MenuNavigationSound : MonoBehaviour
    {
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip _onButtonClickedSoundClip;
        [SerializeField] AudioClip _OnReturnClickedSoundClip;
        [SerializeField] AudioClip _onButtonHighlightSoundClip;

        // TODO Add Logic On button selected
        public void SubscribeAudioSource(AudioSettings audioSettings)
        {
            audioSettings?.SubscribeToSfxAudioSource(_audioSource);
        }
        public void PlayForwardNavigationSound()
        {
            PlayNavigationMenuSound(_onButtonClickedSoundClip);
        }
        public void PlayBackwardNavigationSound()
        {
            PlayNavigationMenuSound(_OnReturnClickedSoundClip);
        }
        public void PlayOnButtonHighlightNavigationSound()
        {
            PlayNavigationMenuSound(_onButtonHighlightSoundClip);
        }
        void PlayNavigationMenuSound(AudioClip clip)
        {
            if (_audioSource == null) return;
            if (clip == null) return;

            MainMenuUI.Instance.OptionMenuUI.AudioSettings.RandomizePitch(_audioSource);

            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}