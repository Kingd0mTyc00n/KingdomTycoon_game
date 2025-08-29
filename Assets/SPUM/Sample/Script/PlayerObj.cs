using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class PlayerObj : MonoBehaviour
{
    public SPUM_Prefabs _prefabs;
    public float _charMS;
    private PlayerState _currentState;

    [SerializeField]
    private AIPath path;

    public Transform _goalPos;
    private bool facingRight = false;
    public bool isAction = false;
    public Dictionary<PlayerState, int> IndexPair = new ();
    void Start()
    {
        if(_prefabs == null )
        {
            _prefabs = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
            if(!_prefabs.allListsHaveItemsExist()){
                _prefabs.PopulateAnimationLists();
            }
        }
        _prefabs.OverrideControllerInit();
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            IndexPair[state] = 0;
        }
        path = GetComponent<AIPath>();
        SetMovePos(_goalPos);
    }
    public void SetStateAnimationIndex(PlayerState state, int index = 0){
        IndexPair[state] = index;
    }
    public void PlayStateAnimation(PlayerState state){
        _prefabs.PlayAnimation(state, IndexPair[state]);
    }
    void Update()
    {
        if(isAction) return;

        //transform.position = new Vector3(transform.position.x,transform.position.y,transform.localPosition.y * 0.01f);
        switch(_currentState)
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
        Flip(_goalPos.position);
        path.destination = _goalPos.position;
    }

    void Flip(Vector3 targetPosition)
    {
        if (targetPosition.x < transform.position.x && facingRight)
        {
            facingRight = false;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (targetPosition.x > transform.position.x && !facingRight)
        {
            facingRight = true;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void SetMovePos(Transform pos)
    {
        isAction = false;
        _goalPos = pos;
        _currentState = PlayerState.MOVE;
    }
}
