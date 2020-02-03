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

        [SerializeField] public float TimeBetweenPunches;

        private CombatTarget _combatTarget;

        private float _punchTimer;

        public string Name { get; set; }

        void Start()
        {
            Name = "Fighter";
            _punchTimer = 0;
        }

        void Update()
        {
            _punchTimer += Time.deltaTime;

            if (_combatTarget != null)
            {
                if (_combatTarget.GetComponent<Health>().IsDead) return;

                var targetPosition = _combatTarget.GetComponent<Transform>().position;
                transform.LookAt(targetPosition);
                GetComponent<Mover>().MoveTo(targetPosition);
                
                if (GetIsInRange(targetPosition))
                {
                    GetComponent<Mover>().Cancel();

                    if (_punchTimer > TimeBetweenPunches)
                    {
                        DoPunch();
                        _punchTimer = 0;
                    }
                }
            }
        }

        private void DoPunch()
        {
            var animator = GetComponent<Animator>();
           
            animator.SetTrigger("OnPunch");
        }

        private void InflictDamage(float damage)
        {
            _combatTarget.GetComponent<Health>().TakeDamage(damage);
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

        //Animation Event
        void Hit()
        {
            InflictDamage(15f);
        }
    }
}
