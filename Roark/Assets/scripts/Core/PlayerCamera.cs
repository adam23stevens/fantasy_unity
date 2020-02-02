using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roark.Core
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField]
        public Transform target;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.position = target.position;
        }
    }

}
