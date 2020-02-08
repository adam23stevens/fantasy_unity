using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roark.Core
{
    public class Patrol : MonoBehaviour
    {
        [SerializeField]
        public bool IsLoopPatrol;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
            {
                var child = transform.GetChild(childIndex);
                Gizmos.DrawSphere(child.transform.position, 0.1f);

                var nextChildIndex = (childIndex + 1) >= transform.childCount ?
                        0 :
                        childIndex + 1;
                var nextChild = transform.GetChild(nextChildIndex);

                if (nextChildIndex == 0 && !IsLoopPatrol) return;                          
               
                Gizmos.DrawLine(child.transform.position, nextChild.transform.position);
            }
        }
    }

}

