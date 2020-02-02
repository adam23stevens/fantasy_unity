using System;
using UnityEngine;
using Roark.Movement;
using Roark.Core;

namespace Roark.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField]
        public int Range;

        private CombatTarget _combatTarget;

        public string Name { get; set; }

        void Start()
        {
            Name = "Fighter";
        }

        void Update()
        {
            if (_combatTarget != null)
            {
                var targetPosition = _combatTarget.GetComponent<Transform>();
                GetComponent<Mover>().MoveTo(targetPosition.position);

                if (GetIsInRange(targetPosition.position))
                {
                    GetComponent<Mover>().Cancel();
                }
            }
        }

        private bool GetIsInRange(Vector3 targetPosition)
        {
            var isInScope = Vector3.Distance(targetPosition, transform.position) < Range;
            return isInScope;
        }

        public void Fight(CombatTarget target)
        {
            GetComponent<Scheduler>().StartAction(this);

            _combatTarget = target;
        }

        public void Cancel()
        {
            _combatTarget = null;
        }
    }
}
