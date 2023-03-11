using MalbersAnimations.Utilities;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Utilities
{
    [AddComponentMenu("Malbers/Effects")]
    public class EffectBehaviour : StateMachineBehaviour
    {
        [Tooltip("Re-check If there any new Effect Manager on the Character (Use this when the Weapons are added or Removed and you want to add effect to them")]
        public bool DynamicManagers = false;
        public List<EffectItem> effects = new List<EffectItem>();
        private  EffectManager[] effectManagers = null;
        private bool HasEfM = false;

        override public void OnStateEnter(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (effectManagers == null || DynamicManagers)
            {
                HasEfM = false;
                effectManagers = anim.GetComponentsInChildren<EffectManager>();

                if (effectManagers != null && effectManagers.Length > 0) HasEfM = true;
            }

            //Reset that the messages were sent. Always on State Enter
            if (HasEfM) 
                foreach (var item in effects) item.sent = false;
           
        }

        override public void OnStateUpdate(Animator anim, AnimatorStateInfo state, int layer)
        {
            if (HasEfM)
            {
                var time = state.normalizedTime % 1;

                foreach (var e in effects)
                {
                    if (!e.sent && (time >= e.Time))
                    {
                        foreach (var EM in effectManagers)
                        {
                            if (e.action == EffectOption.Play)
                                EM.PlayEffect(e.ID);
                            else
                                EM.StopEffect(e.ID);
                            e.sent = true;
                        }
                    }
                }
            }
        }

        override public void OnStateExit(Animator anim, AnimatorStateInfo state, int layer)
        {
            if (HasEfM)
            {
                if (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == state.fullPathHash) return; //means is transitioning to it self

                foreach (var e in effects)
                {
                    if (e.Time == 1 || (e.ExecuteOnExit && !e.sent))
                    {
                        foreach (var EM in effectManagers)
                        {
                            if (e.action == EffectOption.Play)
                            {
                                EM.PlayEffect(e.ID);
                            }
                            else
                            {
                                EM.StopEffect(e.ID);
                            }
                        }
                    }
                    e.sent = true;
                }
            }
        }

        private void OnValidate()
        {
            foreach (var item in effects)
            {
                item.name = $"{item.action} Effect [{item.ID}]";

                if (item.Time == 0)
                    item.name += $"  -  [On Enter]";
                else if (item.Time == 1)
                    item.name += $"  -  [On Exit]";
                else
                    item.name += $"  -  [OnTime] ({item.Time:F2})";

                item.showExecute = item.Time != 1  && item.Time != 0;
            }
        }
    }




    [System.Serializable]
    public class EffectItem
    {
        [HideInInspector] public string name;
        [HideInInspector] public bool showExecute;
        [Tooltip("ID of the Effect")]
        public int ID = 1;                           //ID of the Attack Trigger to Enable/Disable during the Attack Animation
        public EffectOption action = EffectOption.Play;
        [Range(0, 1)]
        public float Time = 0;

        [Tooltip("If the animation is interrupted Play or Stop the Effect on Exit")]
        [Hide(nameof(showExecute))]
        public bool ExecuteOnExit = true;
        public bool sent { get; set; }
    }

    public enum EffectOption { Play, Stop }



#if UNITY_EDITOR

#endif
}