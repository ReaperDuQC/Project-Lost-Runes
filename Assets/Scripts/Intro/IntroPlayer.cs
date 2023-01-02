using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace LostRunes.Intro
{
    public class IntroPlayer : MonoBehaviour
    {
        public Action OnIntroVideoEnded;
        [SerializeField] VideoClip _clip;
        public void PlayIntro()
        {
            float duration = _clip != null ? (float)_clip.length : 0f;
            StartCoroutine(PlayIntroCoroutine(duration));
        }
        IEnumerator PlayIntroCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);

            IntroVideoEnded();
        }
        void IntroVideoEnded()
        {
            if (OnIntroVideoEnded != null)
            {
                OnIntroVideoEnded();
            }
        }
    }
}