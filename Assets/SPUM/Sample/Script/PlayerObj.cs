using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class PlayerObj : MonoBehaviour
{
    [SerializeField]
    private AIPath path;

    public SPUM_Prefabs _prefabs;
    public float _charMS;
    private PlayerState _currentState;


    public Vector3 _goalPos;
    private bool facingRight = false;
    private Vector3 lastPosition;
    public bool isAction = false;
    public Dictionary<PlayerState, int> IndexPair = new();
    void Start()
    {
        if (_prefabs == null)
        {
            _prefabs = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
            if (!_prefabs.allListsHaveItemsExist())
            {
                _prefabs.PopulateAnimationLists();
            }
        }
        _prefabs.OverrideControllerInit();
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            IndexPair[state] = 0;
        }
        path = GetComponent<AIPath>();
        lastPosition = transform.position;
    }
    public void SetStateAnimationIndex(PlayerState state, int index = 0)
    {
        IndexPair[state] = index;
    }
    public void PlayStateAnimation(PlayerState state)
    {
        _prefabs.PlayAnimation(state, IndexPair[state]);
    }
    void Update()
    {
        FlipBasedOnMovement();

        if (isAction) return;

        switch (_currentState)
        {
            case PlayerState.IDLE:

                break;

            case PlayerState.MOVE:
                DoMove();
                break;
        }
        PlayStateAnimation(_currentState);

    }

    void DoMove()
    {
        path.maxSpeed = _charMS;
        path.destination = _goalPos;
        isAction = true;
    }

    // void Flip(Vector3 targetPosition)
    // {
    //     if (targetPosition.x < transform.position.x && facingRight)
    //     {
    //         facingRight = false;
    //         Vector3 scale = transform.localScale;
    //         scale.x *= -1;
    //         transform.localScale = scale;
    //     }
    //     else if (targetPosition.x > transform.position.x && !facingRight)
    //     {
    //         facingRight = true;
    //         Vector3 scale = transform.localScale;
    //         scale.x *= -1;
    //         transform.localScale = scale;
    //     }
    // }

    void FlipBasedOnMovement()
    {
        Vector3 currentPosition = transform.position;
        float movementThreshold = 0.01f;

        if (Vector3.Distance(currentPosition, lastPosition) > movementThreshold)
        {
            if (currentPosition.x < lastPosition.x && facingRight)
            {
                facingRight = false;
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
            else if (currentPosition.x > lastPosition.x && !facingRight)
            {
                facingRight = true;
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }

        // Cập nhật vị trí cuối
        lastPosition = currentPosition;
    }
    public void SetIdle()
    {
        isAction = false;
        _goalPos = transform.position;
        path.canMove = true;
        _currentState = PlayerState.IDLE;
    }
    public void SetMovePos(Vector3 pos)
    {
        isAction = false;
        _goalPos = pos;
        path.canMove = true;
        _currentState = PlayerState.MOVE;
    }
    public void SetAttack()
    {
        isAction = false;
        _goalPos = transform.position;
        path.canMove = false;
        _currentState = PlayerState.ATTACK;
    }
}
