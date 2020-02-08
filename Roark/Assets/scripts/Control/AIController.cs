using System;
using System.Collections;
using System.Collections.Generic;
using Roark.Combat;
using Roark.Core;
using Roark.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace Roark.Control
{
    public class AIController : MonoBehaviour
    {

        [SerializeField]
        public float VisionRange;
        [SerializeField]
        public float VisionAngle;
        [SerializeField]
        public float SuspiciionTime;
        [SerializeField]
        public Patrol PatrolLine;
        [SerializeField]
        public float PatrolPointDwellTime;
        [SerializeField]
        public float AttackSpeed;
        [SerializeField]
        public LayerMask ViewMask;

        private bool _spottedPlayer;
        private UnityEngine.Vector3 _startPosition;
        private Quaternion _rotatePosition;
        private bool _isPatrolling;
        private bool _isSuspicious;
        private float _timeSinceSawLastPlayer;
        private float _timeDwellingAtPatrolPoint;
        private bool _hasAttackedPlayer;
        private GameObject _player;
        private Health _health;
        private Collider _spotCollider;
        private int _patrolPointIndex;
        private bool _patrolBackwards;
        private float _patrolSpeed;
        private Mover _mover;
        private Fighter _fighter;
        private NavMeshAgent _navMeshAgent;

        // Start is called before the first frame update
        void Start()
        {
            _spottedPlayer = false;
            _isSuspicious = false;
            _isPatrolling = true;
            _timeSinceSawLastPlayer = Mathf.Infinity;
            _startPosition = transform.position;
            _rotatePosition = transform.rotation;
            _player = GameObject.FindWithTag("Player");
            _hasAttackedPlayer = false;
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _patrolSpeed = _navMeshAgent.speed;
            _patrolBackwards = false; 
            GetSpotCollider();
            _patrolPointIndex = 0;
            _timeDwellingAtPatrolPoint = 0f;
        }

        // Update is called once per frame
        void Update()
        {
         
            if (_health.IsDead) return;

            PatrolBehaviour();

            LookoutForPlayer();

            if (_spottedPlayer)
            {
                AttackBehaviour();
            }

            else if (_hasAttackedPlayer)
            {
                SuspicionBehavior();
            }

            UpdateTimer();
        }

        private void UpdateTimer()
        {
            _timeSinceSawLastPlayer += Time.deltaTime;
            _timeDwellingAtPatrolPoint += Time.deltaTime;
        }

        private void SuspicionBehavior()
        {
            _mover.StartMoveAction(transform.position);

            if (_timeSinceSawLastPlayer > SuspiciionTime)
            {
                _isSuspicious = false;
                _hasAttackedPlayer = false;
            }
        }

        private void AttackBehaviour()
        {
            _navMeshAgent.speed = AttackSpeed;
            _fighter.Fight(_player);
            _hasAttackedPlayer = true;
        }

        private void PatrolBehaviour()
        {
            if (PatrolLine != null)
            {
                if (_patrolPointIndex >= PatrolLine.transform.childCount)
                {
                    if (PatrolLine.IsLoopPatrol)
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
                
                var targetPoint = PatrolLine.transform.GetChild(_patrolPointIndex);

                if (targetPoint != null)
                {
                    _mover.MoveTo(targetPoint.transform.position);
                    _navMeshAgent.speed = _patrolSpeed;

                    if (Vector3.Distance(transform.position, targetPoint.transform.position) < 1f)
                    {
                        _timeDwellingAtPatrolPoint = 0f;
                        DwellAtPatrolPoint();
                        
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
            } else
            {
                if (!_spottedPlayer && !_isSuspicious)
                {
                    _mover.MoveTo(_startPosition);
                    if (Vector3.Distance(transform.position, _startPosition) < 0.2f)
                    {
                        transform.rotation = _rotatePosition;
                    }
                }
            }
        }

        private void DwellAtPatrolPoint()
        {
            if (_timeDwellingAtPatrolPoint < PatrolPointDwellTime)
            {
                _mover.MoveTo(transform.position);
            }
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
                Vector3 dirToPlayer = (_player.transform.position - transform.position).normalized;
                float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
                
                if (angleBetweenGuardAndPlayer < VisionAngle)
                {
                    if (!Physics.Linecast(transform.position, _player.transform.position, ViewMask))
                    {
                        return true;
                    }
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

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, VisionRange);
        }
    }
}


