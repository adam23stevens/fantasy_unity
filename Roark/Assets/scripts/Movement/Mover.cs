using System.Collections;
using System.Collections.Generic;
using Roark.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Roark.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        public string Name { get; set; }

        void Start()
        {
            Name = "Mover";
        }

        public void Cancel()
        {
            GetComponent<NavMeshAgent>().isStopped = true;
        }

        public void StartMoveAction(Vector3 target)
        {
            GetComponent<Scheduler>().StartAction(this);

            MoveTo(target);
        }

        public void MoveTo(Vector3 target)
        {
            GetComponent<NavMeshAgent>().isStopped = false;
            GetComponent<NavMeshAgent>().destination = target;
        }

        private void LateUpdate()
        {
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 meshVelocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localMeshVelocity = transform.InverseTransformDirection(meshVelocity);
            float speed = localMeshVelocity.z;

            GetComponent<Animator>().SetFloat("Forward", speed);
        }
    }

}

