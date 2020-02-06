using System;
using System.Collections;
using System.Collections.Generic;
using Roark.Combat;
using Roark.Movement;
using UnityEngine;

public class AIController : MonoBehaviour
{

    [SerializeField]
    public float VisionRange;
    [SerializeField]
    public float SuspiciionTime;

    private bool _spottedPlayer;
    private Vector3 _startPosition;
    private bool _isSuspicious;
    private float _timeSinceSawLastPlayer;
    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _spottedPlayer = false;
        _isSuspicious = false;
        _timeSinceSawLastPlayer = Mathf.Infinity;
        _startPosition = transform.position;
        _player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Health>().IsDead) return;

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
        GetComponent<Mover>().StartMoveAction(transform.position);
        if (_timeSinceSawLastPlayer > SuspiciionTime)
        {
            GetComponent<Mover>().StartMoveAction(_startPosition);
        }
    }

    private void AttackBehaviour()
    {
        GetComponent<Fighter>().Fight(_player);
    }

    private void PatrolBehaviour()
    {
        LookoutForPlayer();

        if (!_spottedPlayer && !_isSuspicious)
        {
            _isSuspicious = true;
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
            Ray ray = new Ray(transform.position, _player.transform.position);
            var rayCastHits = Physics.RaycastAll(ray);
            print(rayCastHits);
            foreach(RaycastHit hit in Physics.RaycastAll(ray))
            {
                if (hit.collider.isTrigger)
                {
                    print("hit");
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, VisionRange);
    }
}
