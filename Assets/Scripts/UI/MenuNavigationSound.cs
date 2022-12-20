using LostRunes.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class MenuNavigationSound : MonoBehaviour
    {
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip _forwardNavigationSoundClip;
        [SerializeField] AudioClip _backwardNavigationSoundClip;
        [SerializeField] AudioClip _onButtonHighlightSoundClip;

        // TODO Add Logic On button selected
        public void SubscribeAudioSource()
        {
            MainMenuUI.Instance.OptionMenuUI.AudioSettings.SubscribeToSfxAudioSource(_audioSource);
        }
        public void PlayForwardNavigationSound()
        {
            PlayNavigationMenuSound(_forwardNavigationSoundClip);
        }
        public void PlayBackwardNavigationSound()
        {
            PlayNavigationMenuSound(_backwardNavigationSoundClip);
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