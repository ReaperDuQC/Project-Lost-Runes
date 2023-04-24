using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetcodePlus
{
    /// <summary>
    /// Object will be spawned/despawned based on distance to SNetworkPlayer
    /// </summary>

    [RequireComponent(typeof(SNetworkObject))]
    public class SNetworkOptimizer : MonoBehaviour
    {
        [Header("Optimization")]
        public float active_range = 50f; //If farther than this, will be disabled for optim
        public bool always_run_scripts = false; //Set to true to have other scripts still run when this one is not active
        public bool turn_off_gameobject = false; //Set to true if you want the gameobject to be SetActive(false) when far away

        private SNetworkObject nobj;
        private Transform transf;
        private bool is_active = true;

        private List<MonoBehaviour> scripts = new List<MonoBehaviour>();
        private List<Animator> animators = new List<Animator>();

        private static List<SNetworkOptimizer> opt_list = new List<SNetworkOptimizer>();

        void Awake()
        {
            opt_list.Add(this);
            transf = transform;
            nobj = GetComponent<SNetworkObject>();
            scripts.AddRange(GetComponents<MonoBehaviour>());
            animators.AddRange(GetComponentsInChildren<Animator>());
        }

        private void Start()
        {
            if (nobj.IsSceneObject)
            {
                nobj.RegisterScene(); //Register scene now in case this Start function is called before SNetworkObject start
                SetActive(false); //Prevent spawn scene objects (SpawnOrDestroy function), wait for the optimizer to spawn the ones near the player instead
            }
        }

        void OnDestroy()
        {
            opt_list.Remove(this);
        }

        public void SetActive(bool visible)
        {
            if (is_active != visible)
            {
                is_active = visible;

                if (!always_run_scripts && !turn_off_gameobject)
                {
                    foreach (MonoBehaviour script in scripts)
                    {
                        if (script != null && script != this && script != nobj)
                            script.enabled = visible;
                    }

                    foreach (Animator anim in animators)
                    {
                        if (anim != null)
                            anim.enabled = visible;
                    }
                }

                if (visible)
                    NetObject.Spawn();
                else
                    NetObject.Despawn();

                if (turn_off_gameobject)
                    gameObject.SetActive(visible);
            }
        }

        public Vector3 GetPos()
        {
            return transf.position;
        }

        public bool IsActive()
        {
            return is_active;
        }

        public SNetworkObject NetObject { get { return nobj; } }

        public static List<SNetworkOptimizer> GetAll()
        {
            return opt_list;
        }
    }
}
