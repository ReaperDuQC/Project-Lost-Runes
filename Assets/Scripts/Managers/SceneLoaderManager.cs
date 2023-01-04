using LostRunes.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LostRunes
{
    public class SceneLoaderManager : MonoBehaviour
    {
        [SerializeField] SceneTransition _sceneTransition;
        [SerializeField] string _sceneToLoad;

        public Action _progressPaused;

        [Header("Async Options")]
        [SerializeField] bool _useAsyncLoading;
        [SerializeField] bool _requirePrompt;
        [SerializeField] Button _promptButtons;
        [SerializeField] bool _displayProgress;
        [SerializeField] TextMeshProUGUI _progressText;

        bool _asyncProgressResumed = false;
        bool _transitionInOver = false;
        bool _transitionOutOver = false;
        bool _readyToLoad = false;
        private void Start()
        {
            if(_sceneTransition != null)
            {
                _sceneTransition._transitionInOver += TransitionInIsOver;
                _sceneTransition._transitionOutOver += TransitionOutIsOVer;
            }
        }
        public void LoadScene()
        {
           StartCoroutine(LoadSceneCoroutine());
        }
        IEnumerator LoadSceneCoroutine()
        {
            yield return new WaitUntil(() => _transitionInOver == true);
            yield return new WaitUntil(() => _readyToLoad == true);

            // start transition out
            _sceneTransition.StartOutTransition();
            yield return new WaitUntil(() => _transitionOutOver == true);
            // on transition out over == Load scene 

            if (_useAsyncLoading)
            {
                AsyncOperation async = SceneManager.LoadSceneAsync(_sceneToLoad);
                async.allowSceneActivation = false;
                // linked ??
                bool linked = false;

                if (_displayProgress)
                {
                    DisplayProgress(true);
                }

                if (_requirePrompt)
                {
                    _promptButtons.onClick.AddListener(ResumeAsyncProgress);
                    _progressPaused += HideProgressAndDisplayPrompt;

                }
                else
                {
                    _progressPaused += ResumeAsyncProgress;
                }

                while (!async.isDone)
                {
                    float progress = Mathf.Clamp01(async.progress / 0.9f);
                    if (_displayProgress)
                    {
                        UpdateProgressText(progress);
                    }

                    if (progress >= 1f && !linked)
                    {
                        linked = true;

                        if (_progressPaused != null)
                        {
                            _progressPaused();
                        }
                    }

                    if (_asyncProgressResumed)
                    {
                        async.allowSceneActivation = true;
                    }

                    yield return null;
                }
            }
            else
            {
                if (_requirePrompt)
                {
                    if(_promptButtons != null)
                    {

                        _promptButtons.onClick.AddListener(LoadSceneToLoad);
                        DisplayPrompt(true);
                    }
                }
                else
                {
                    LoadSceneToLoad();
                }
            }
        }

        private void HideProgressAndDisplayPrompt()
        {
            DisplayProgress(false);
            DisplayPrompt(true);
        }

        private void UpdateProgressText(float progress)
        {
            if (_progressText != null)
            {
                _progressText.text = ((int)(progress * 100f)).ToString();
            }
        }

        private void LoadSceneToLoad()
        {
            SceneManager.LoadScene(_sceneToLoad);
        }
        void ResumeAsyncProgress()
        {
            _asyncProgressResumed = true;
        }
        public void ReadyToLoad()
        {
            _readyToLoad = true;
        }
        public void TransitionInIsOver()
        {
            _transitionInOver = true;
        }
        public void TransitionOutIsOVer()
        {
            _transitionOutOver = true;
        }
        public void SetSceneToLoad(string sceneToLoad)
        {
            _sceneToLoad = sceneToLoad;
        }
        void DisplayPrompt(bool display)
        {
            if (_promptButtons == null) return;

            _promptButtons.gameObject.SetActive(display);
        }
        void DisplayProgress(bool display)
        {
            if (_progressText == null) return;

            _progressText.gameObject.SetActive(display);
        }
        public void ReturnToMainMenu()
        {
            SetSceneToLoad("MainMenu");
            _requirePrompt = false;
            _displayProgress = false;

            LoadScene();
        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}