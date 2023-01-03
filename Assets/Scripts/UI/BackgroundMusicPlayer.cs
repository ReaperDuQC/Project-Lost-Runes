using LostRunes.Menu;
using UnityEngine;

namespace LostRunes
{
    public class BackgroundMusicPlayer : MonoBehaviour
    {
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip _backgroundMusicClip;
        public void SubscribeAudioSource()
        {
            MainMenuUI.Instance.OptionMenuUI.AudioSettings.SubscribeToMusicAudioSource(_audioSource);
        }
        public void InitializeAudioSource()
        {
            if (_audioSource == null) return;
            if (_backgroundMusicClip == null) return;

            _audioSource.playOnAwake = false;
            _audioSource.clip = _backgroundMusicClip;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
}