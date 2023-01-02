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
            WaitForNetworkManager();
        }

        private void LinkIntroPlayerToSceneManager()
        {
            if (_introPlayer == null) return;
            if (_sceneLoaderManager == null) return;

            _introPlayer.OnIntroVideoEnded += _sceneLoaderManager.LoadNextScene;
        }

        void WaitForNetworkManager()
        {
            StartCoroutine(WaitForNetworkManagerCoroutine());
        }
        IEnumerator WaitForNetworkManagerCoroutine()
        {
            yield return new WaitUntil(() => NetworkManager.Singleton != null);

            if (_sceneLoaderManager != null)
            {
                _sceneLoaderManager.ReadyToLoad();
            }
        }
    }
}