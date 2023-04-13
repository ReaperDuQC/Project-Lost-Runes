using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            Destroy(this);
        }
    }
}