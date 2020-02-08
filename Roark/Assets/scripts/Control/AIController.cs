using System;
using System.Collections;
using System.Collections.Generic;
using Roark.Combat;
using Roark.Core;
using Roark.Movement;
using UnityEngine;

namespace Roark.Control
{
    public class AIController : MonoBehaviour
    {

        [SerializeField]
        public float VisionRange;
        [SerializeField]
        public float SuspiciionTime;
        [SerializeField]
        public Patrol patrol;

        private bool _spottedPlayer;
        private Vector3 _startPosition;
        private bool _isSuspicious;
        private float _timeSinceSawLastPlayer;
        private GameObject _player;
        private Health _health;
        private Collider _spotCollider;
        private int _patrolPointIndex;
        private bool _patrolBackwards;
        private Mover _mover;
        private Fighter _fighter;

        // Start is called before the first frame update
        void Start()
        {
            _spottedPlayer = false;
            _isSuspicious = false;
            _timeSinceSawLastPlayer = Mathf.Infinity;
            _startPosition = transform.position;
            _player = GameObject.FindWithTag("Player");
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _patrolBackwards = false; 
            GetSpotCollider();
            _patrolPointIndex = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (_health.IsDead) return;

            PatrolBehaviour();

            if (_spottedPlayer)
            {
                AttackBehaviour();
            }
            else if (_isSuspicious)
            {
                BeSuspicious();
            }

            UpdateTimer();
        }

        private void UpdateTimer()
        {
            _timeSinceSawLastPlayer += Time.deltaTime;
        }

        private void BeSuspicious()
        {
            _mover.StartMoveAction(transform.position);
            if (_timeSinceSawLastPlayer > SuspiciionTime)
            {
                _mover.StartMoveAction(_startPosition);
            }
        }

        private void AttackBehaviour()
        {
            _fighter.Fight(_player);
        }

        private void PatrolBehaviour()
        {
            if (patrol != null)
            {
                if (_patrolPointIndex >= patrol.transform.childCount)
                {
                    if (patrol.IsLoopPatrol)
                    {
                        _patrolPointIndex = 0;
                    }
                    else
                    {
                        _patrolBackwards = true;
                        _patrolPointIndex--;
                    }
                }
                else if (_patrolPointIndex == 0 && _patrolBackwards)
                {
                    _patrolBackwards = false;
                }
                
                var targetPoint = patrol.transform.GetChild(_patrolPointIndex);

                if (targetPoint != null)
                {
                    _mover.MoveTo(targetPoint.transform.position);

                    if (Vector3.Distance(transform.position, targetPoint.transform.position) < 1f)
                    {
                     
                        if (_patrolBackwards)
                        {
                            _patrolPointIndex = (_patrolPointIndex == 0)
                                ? 1 :
                                _patrolPointIndex - 1;
                        }
                        else
                        {
                            _patrolPointIndex++;
                        }
                    }
                }
            }

            LookoutForPlayer();

            //TODO - currently always suspicious 
            /*if (!_spottedPlayer && !_isSuspicious)
            {
                _isSuspicious = true;
            }*/
        }

        private void LookoutForPlayer()
        {
            _spottedPlayer = CanSpotPlayer();

            if (_spottedPlayer)
            {
                _timeSinceSawLastPlayer = 0f;
            } 
        }

        private bool CanSpotPlayer()
        {
            var isInRange = Vector3.Distance(transform.position, _player.transform.position) < VisionRange;
            
            if (isInRange)
            {
                Ray ray = new Ray(transform.position, _player.transform.position);
                
                foreach (RaycastHit hit in Physics.RaycastAll(ray))
                {
                    return hit.collider == _spotCollider;
                }
            }

            return false;
        }

        private void GetSpotCollider()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var collider = child.GetComponent<BoxCollider>();
                if (collider.isTrigger)
                {
                    
                    _spotCollider = collider;
                }
            }

            print(_spotCollider);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, VisionRange);
        }
    }
}


