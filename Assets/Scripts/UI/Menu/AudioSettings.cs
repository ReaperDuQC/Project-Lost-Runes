using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace LostRunes.Menu
{
    [System.Serializable]
    public class AudioSettingsData
    {
        public float _globalVolume = 1f;
        public float _musicVolume = 1f;
        public float _sfxVolume = 1f;
        public float _voiceVolume = 1f;

        public AudioSettingsData()
        {
            _globalVolume = 1f;
            _musicVolume = 1f;
            _sfxVolume = 1f;
            _voiceVolume = 1f;
        }
        public AudioSettingsData(AudioSettings soundSettings)
        {
            _globalVolume = soundSettings.GlobalVolume;
            _musicVolume = soundSettings.MusicVolume;
            _sfxVolume = soundSettings.SFXVolume;
            _voiceVolume = soundSettings.VoiceVolume;
        }
    }
    public class AudioSettings : MonoBehaviour
    {
        [SerializeField] Slider _globalVolumeSlider;
        public float GlobalVolume { get { return _globalVolume; } }
        float _globalVolume = 1f;
        [SerializeField] Slider _musicVolumeSlider;
        public float MusicVolume { get { return _musicVolume; } }
        float _musicVolume = 1f;
        [SerializeField] Slider _sfxVolumeSlider;
        public float SFXVolume { get { return _sfxVolume; } }
        float _sfxVolume = 1f;
        [SerializeField] Slider _voiceVolumeSlider;
        public float VoiceVolume { get { return _voiceVolume; } }
        float _voiceVolume = 1f;

        [SerializeField] TextMeshProUGUI _globalVolumeValue;
        [SerializeField] TextMeshProUGUI _musicVolumeValue;
        [SerializeField] TextMeshProUGUI _sfxVolumeValue;
        [SerializeField] TextMeshProUGUI _voiceVolumeValue;

        [SerializeField] List<AudioSource> _musicAudioSource = new List<AudioSource>();
        [SerializeField] List<AudioSource> _sfxAudioSource = new List<AudioSource>();
        [SerializeField] List<AudioSource> _voiceAudioSource = new List<AudioSource>();
        public void SubscribeToMusicAudioSource(AudioSource audioSource)
        {
            _musicAudioSource.Add(audioSource);
        }
        public void SubscribeToSfxAudioSource(AudioSource audioSource)
        {
            _sfxAudioSource.Add(audioSource);
        }
        public void SubscribeToVoiceAudioSource(AudioSource audioSource)
        {
            _voiceAudioSource.Add(audioSource);
        }
        public void LoadAudioSettingsData()
        {
            AudioSettingsData data = SaveSystem.SaveSystem.LoadAudioSettings();

            _globalVolume = data._globalVolume;
            _musicVolume = data._musicVolume;
            _sfxVolume = data._sfxVolume;
            _voiceVolume = data._voiceVolume;

            UpdateAllSLiders();
        }
        void UpdateAllSLiders()
        {
            UpdateSlider(_globalVolumeSlider, _globalVolume, _globalVolumeValue);
            UpdateSlider(_musicVolumeSlider, _musicVolume, _musicVolumeValue);
            UpdateSlider(_sfxVolumeSlider, _sfxVolume, _sfxVolumeValue);
            UpdateSlider(_voiceVolumeSlider, _voiceVolume, _voiceVolumeValue);
            UpdateGlobalVolume(_globalVolume);
        }
        public void SaveAudioSettings()
        {
            SaveSystem.SaveSystem.SaveAudioSettings(this);
        }
        void UpdateSlider(Slider slider, float value, TextMeshProUGUI text)
        {
            bool sameValue = value == slider.value;
            slider.value = value;

            UpdateText(text, value);
        }
        public void UpdateVolume(float value, ref float current, TextMeshProUGUI text)
        {
            current = value;
            UpdateText(text, current);
        }
        void UpdateText(TextMeshProUGUI text, float value)
        {
            text.text = ((int)(value * 100f)).ToString();
        }
        public void UpdateGlobalVolume(float value)
        {
            UpdateVolume(value, ref _globalVolume, _globalVolumeValue);
            ApplyGlobalVolume();
        }
        public void UpdateMusicVolume(float value)
        {
            UpdateVolume(value, ref _musicVolume, _musicVolumeValue);
            ApplyMusicVolume();
        }
        public void UpdateSFXVolume(float value)
        {
            UpdateVolume(value, ref _sfxVolume, _sfxVolumeValue);
            ApplySFXVolume();
        }
        public void UpdateVoiceVolume(float value)
        {
            UpdateVolume(value, ref _voiceVolume, _voiceVolumeValue);
            ApplyVoiceVolume();
        }
        public void ResetToDefault()
        {
            _globalVolumeSlider.value = 1f;
            _musicVolumeSlider.value = 1f;
            _sfxVolumeSlider.value = 1f;
            _voiceVolumeSlider.value = 1f;
            SaveAudioSettings();
        }
        void ApplyGlobalVolume()
        {
            ApplySFXVolume();
            ApplyMusicVolume();
            ApplyVoiceVolume();
        }
        void ApplySFXVolume()
        {
            float volume = _sfxVolume * _globalVolume;

            ApplyVolumeToAudioSources(_sfxAudioSource, volume);
        }
        void ApplyMusicVolume()
        {
            float volume = _musicVolume * _globalVolume;

            ApplyVolumeToAudioSources(_musicAudioSource, volume);
        }
        void ApplyVoiceVolume()
        {
            float volume = _voiceVolume * _globalVolume;

            ApplyVolumeToAudioSources(_voiceAudioSource, volume);
        }
        void ApplyVolumeToAudioSources(List<AudioSource> audioSources, float volume)
        {
            foreach (AudioSource source in audioSources)
            {
                ApplyVolumeToAudioSource(source, volume);
            }
        }
        void ApplyVolumeToAudioSource(AudioSource source, float volume)
        {
            if (source == null) return;

            source.volume = volume;
        }
        public static void RandomizePitch(AudioSource source)
        {
            if (source == null) return;

            source.pitch = 1.0f + Random.Range(-0.1f, 0.1f);
        }
    }
}