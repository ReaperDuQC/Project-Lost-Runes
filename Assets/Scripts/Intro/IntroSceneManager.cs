using LostRunes.Intro;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace LostRunes
{
    public class IntroSceneManager : MonoBehaviour
    {
        [SerializeField] SceneLoaderManager _sceneLoaderManager;
        [SerializeField] IntroPlayer _introPlayer;

        private void Start()
        {
            LinkIntroPlayerToSceneManager();
            _introPlayer.PlayIntro();
        }

        private void LinkIntroPlayerToSceneManager()
        {
            if (_introPlayer == null) return;
            if (_sceneLoaderManager == null) return;

            _introPlayer.OnIntroVideoEnded += _sceneLoaderManager.LoadScene;
            _introPlayer.OnIntroVideoEnded += SetReadyToLoad;
        }
        private void SetReadyToLoad()
        {
            _sceneLoaderManager.ReadyToLoad();
        }
    }
}