using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        private void Awake()
        {
            StartInTransition();
        }
        public void StartInTransition()
        {
            StartCoroutine(StartTransition(true));
        }
        public void StartOutTransition()
        {
            StartCoroutine(StartTransition(false));
        }
        IEnumerator StartTransition(bool isInTransition)
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
                    SetPanelOpacity(isInTransition ? 1f - ratio : ratio);
                    yield return null;
                }
            }

            if(isInTransition)
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
        void SetPanelOpacity(float opacityValue)
        {
            Color color = _transitionPanel.color;
            color.a = opacityValue;
            _transitionPanel.color = color;
        }
    }
}
