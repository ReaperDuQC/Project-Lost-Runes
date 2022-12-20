using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator _animator;
        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            _animator.applyRootMotion = isInteracting;
            _animator.SetBool("IsInteracting", isInteracting);
            _animator.CrossFade(targetAnim, 0.2f);
        }
    }
}