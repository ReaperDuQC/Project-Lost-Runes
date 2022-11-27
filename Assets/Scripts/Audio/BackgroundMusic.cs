using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class BackgroundMusic : MonoBehaviour
    {
        [SerializeField] AudioSource _backgroundSounrce;
        [SerializeField] AudioClip _backgroundMusic;
        private void Awake()
        {
            _backgroundSounrce = GetComponent<AudioSource>();
            InitializeAudioSource();
        }
        private void Start()
        {
            if (_backgroundMusic != null)
            {
                GameManager.Instance.GameMenu.AudioSettings.SubscribeToMusicAudioSource(_backgroundSounrce);
            }
        }
        void InitializeAudioSource()
        {
            if (_backgroundMusic == null) return;
            if (_backgroundMusic == null) return;

            _backgroundSounrce.playOnAwake = false;
            _backgroundSounrce.clip = _backgroundMusic;
            _backgroundSounrce.loop = true;
            _backgroundSounrce.Play();
        }
    }
}