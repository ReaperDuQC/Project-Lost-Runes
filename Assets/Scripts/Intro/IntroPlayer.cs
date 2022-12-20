using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace LostRunes.Intro
{
    public class IntroPlayer : MonoBehaviour
    {
        [SerializeField] VideoClip _clip;
        [SerializeField] string _sceneToLoad;
        void Start()
        {

            float duration = _clip != null ? (float)_clip.length : 0f;

            StartCoroutine(DelayedChangeScene(duration));
        }

        IEnumerator DelayedChangeScene(float duration)
        {
            yield return new WaitForSeconds(duration);

            SceneManager.LoadScene(_sceneToLoad);
        }
    }
}