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
        public float HearingRange;
        //[SerializeField]
        //public float ChaseLength;
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
        [SerializeField]
        public Transform FollowPoint;

        private bool _spottedPlayer;
        private Vector3 _startPosition;
        private Quaternion _rotatePosition;
        private bool _isSuspicious;
        private float _timeSinceSawLastPlayer;
        private float _timeDwellingAtPatrolPoint;
        private bool _hasAttackedPlayer;
        //private float _chaseTime;
        private GameObject _player;
        private Health _health;
        private int _currentPatrolPointIndex;
        private int _nextPatrolPointIndex;
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
            _currentPatrolPointIndex = 0;
            _timeDwellingAtPatrolPoint = 0f;
            //_chaseTime = 0f;
        }

        // Update is called once per frame
        void Update()
        {

            if (_health.IsDead) return;

            PatrolBehaviour();

            LookoutForPlayer();

            if (_spottedPlayer || CanHearPlayer())
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
            //_chaseTime += Time.deltaTime;
        }

        private void SuspicionBehavior()
        {
            _mover.StartMoveAction(_player.transform.position);

            if (_timeSinceSawLastPlayer > SuspiciionTime)
            {
                _isSuspicious = false;
                _hasAttackedPlayer = false;
                _navMeshAgent.speed = _patrolSpeed;
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
                if (_currentPatrolPointIndex == PatrolLine.transform.childCount - 1)
                {
                    if (PatrolLine.IsLoopPatrol)
                    {
                        _nextPatrolPointIndex = 0;
                    }
                    else
                    {
                        _patrolBackwards = true;
                        _nextPatrolPointIndex = _currentPatrolPointIndex - 1;
                    }
                }
                else if (_currentPatrolPointIndex == 0 && _patrolBackwards)
                {
                    _patrolBackwards = false;
                    _nextPatrolPointIndex = _currentPatrolPointIndex + 1;
                }
                else
                {
                    _nextPatrolPointIndex = _currentPatrolPointIndex + (_patrolBackwards ? -1 : 1);
                }

                
                var currentPoint = PatrolLine.transform.GetChild(_currentPatrolPointIndex);

                if (currentPoint != null)
                {                                        
                    if (Vector3.Distance(transform.position, currentPoint.transform.position) < 1f)
                    {
                        _timeDwellingAtPatrolPoint = 0;

                        _currentPatrolPointIndex = _nextPatrolPointIndex;
                    }

                    if (_timeDwellingAtPatrolPoint > PatrolPointDwellTime)
                    {
                        _mover.StartMoveAction(currentPoint.transform.position);
                    }
                }
            }
            else if (FollowPoint != null)
            {
                _mover.StartMoveAction(FollowPoint.transform.position);
            }
            else
            {
                if (!_spottedPlayer && !_isSuspicious)
                {
                    _mover.StartMoveAction(_startPosition);
                    if (Vector3.Distance(transform.position, _startPosition) < 0.2f)
                    {
                        transform.rotation = _rotatePosition;
                    }
                }
            }
        }

        private void LookoutForPlayer()
        {
            _spottedPlayer = CanSpotPlayer();
            print(_spottedPlayer);
            if (_spottedPlayer)
            {
                _timeSinceSawLastPlayer = 0f;
                //_chaseTime = 0f;
            }
        }

        private bool CanSpotPlayer()
        {
            var isInRange = Vector3.Distance(transform.position, _player.transform.position) < VisionRange;

            if (isInRange)
            {
                Vector3 dirToPlayer = (_player.transform.position - transform.position).normalized;
                float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
                print(angleBetweenGuardAndPlayer);
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

        private bool CanHearPlayer()
        {
            var isInRange = Vector3.Distance(transform.position, _player.transform.position) < HearingRange;

            if (isInRange)
            {
                var playerNavMeshAgent = _player.GetComponent<NavMeshAgent>();
                return (playerNavMeshAgent.velocity.x > 0f || playerNavMeshAgent.velocity.z > 0f) && Input.GetKey(KeyCode.S) ;
            }
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, VisionRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, HearingRange);
        }
    }
}


