using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeNull.UnityMinimaps.Demo
{
    public class FollowObject : MonoBehaviour
    {
        public GameObject Target;
        public Vector3 Offset;

        private void LateUpdate()
        {
            if (Target != null)
                transform.position = Target.transform.position + Offset;
        }
    }

}