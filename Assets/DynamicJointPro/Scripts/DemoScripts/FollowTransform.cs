using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshqarApps.DynamicJoint
{
    public class FollowTransform : MonoBehaviour
    {
        public Transform followTarget;

        Vector3 prevPos;

        // Start is called before the first frame update
        void Start()
        {
            prevPos = followTarget.position;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Vector3 delta = followTarget.position - prevPos;
            //delta.y = 0;
            this.transform.position += delta;
            prevPos = followTarget.position;
        }
    }
}
