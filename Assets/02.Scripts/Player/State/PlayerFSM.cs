using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utils;

// PlayerFSM 클래스
public class PlayerFSM
{
    public IPlayerState currentState;
    private Dictionary<Type, IPlayerState> _stateCache = new();

    public PlayerMovementController movementController;
    public PlayerCombatController combatController;
    public PlayerInputAction inputActions;

    public PlayerFSM(PlayerMovementController movementController, PlayerCombatController combatController, PlayerInputAction inputActions)
    {
        this.movementController = movementController;
        this.combatController = combatController;
        this.inputActions = inputActions;
    }

    // 상태 변경 시 State 캐싱
    public void ChangeState<T>(params object[] args) where T : IPlayerState
    {
        // 현재 상태에서 Exit
        currentState?.ExitState(this);

        if(_stateCache.ContainsKey(typeof(T)))
        {
            currentState = _stateCache[typeof(T)];
        }else{
            Debug.Log($"{args.Length} : {args}");
            currentState = (T)Activator.CreateInstance(typeof(T), args);
            _stateCache[typeof(T)] = currentState;
        }

        currentState.EnterState(this);
    }

    public void Update()
    {
        if (currentState != null)
            currentState.UpdateState(this);
    }
}
