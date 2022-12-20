using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace LostRunes.Menu
{ 
    public class SceneTransition : MonoBehaviour
    {
        public delegate void OnTransitionOver();
        public OnTransitionOver _transitionInOver;
        public OnTransitionOver _transitionOutOver;

        [SerializeField] Image _transitionPanel;
        [SerializeField] float _transitionDuration = 2f;

        [SerializeField] GameObject _promptButtons;

        [SerializeField] TextMeshProUGUI _progressText;

        bool _isResumed = false;

        string _sceneToLoad = "Sandbox";
        private void Awake()
        {
            EnableProgressText(false);

            StartInTransition();

            _transitionOutOver += LoadScene;
            _transitionInOver += HideTransitionPanel;
        }
        public GameObject GetPromptButton()
        {
            return _promptButtons;
        } 
        public void StartInTransition()
        {
            StartCoroutine(StartTransition(true));
        }
        public void StartOutTransition()
        {
            StartCoroutine(StartTransition(false));
        }
        IEnumerator StartTransition(bool inTransition)
        {
            if (_transitionPanel != null)
            {
                _transitionPanel.gameObject.SetActive(true);

                float currentDuration = 0;
                float ratio = 0f;

                while(ratio < 1f)
                {  
                    currentDuration += Time.deltaTime;
                    ratio = currentDuration / _transitionDuration;
                    SetPanelOpacity(inTransition ? 1f - ratio : ratio);
                    yield return null;
                }
            }

            if(inTransition)
            {
                if(_transitionInOver != null)
                {
                    _transitionInOver();
                }
            }
            else
            {
                if (_transitionOutOver != null)
                {
                    _transitionOutOver();
                }
            }
        }
        void HideTransitionPanel()
        {
            _transitionPanel.gameObject.SetActive(false);
        }
        void SetPanelOpacity(float opacityValue)
        {
            Color color = _transitionPanel.color;
            color.a = opacityValue;
            _transitionPanel.color = color;
        }
        public void DisplayPrompt()
        {
            if (_promptButtons == null) return;

            _promptButtons.SetActive(true);
        }
        public void HidePrompt()
        {
            if (_promptButtons == null) return;

            _promptButtons.SetActive(false);
        }
        public void SetSceneToLoad(string sceneToLoad)
        {
            _sceneToLoad = sceneToLoad;
        }
        public void LoadScene()
        {
            StartCoroutine(AsyncLoadScene(_sceneToLoad));
        }
        public IEnumerator AsyncLoadScene(string sceneToLoad)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneToLoad);
            async.allowSceneActivation = false;

            bool linked = false;

            while (!async.isDone)
            {
                float progress = Mathf.Clamp01(async.progress / 0.9f);

                if(progress >= 1f && !linked)
                {
                    linked = true;
                    EnableProgressText(true);
                }

                if(_isResumed)
                {
                    async.allowSceneActivation = true;
                }

                yield return null;
            }
        }
        public void ResumeAsyncLoad()
        {
            _isResumed = true;
        }
        void EnableProgressText(bool enabled)
        {
            if(_progressText == null) return;

            _progressText.gameObject.SetActive(enabled);    
        }
    }
}
