using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utils;

// PlayerFSM 클래스
public class PlayerFSM
{
    public IPlayerState currentState;
    public PlayerMovementController movementController;
    public PlayerCombatController combatController;
    public PlayerCameraController cameraController;
    public PlayerInputAction inputActions;

    public PlayerFSM(PlayerMovementController movementController, PlayerCombatController combatController, PlayerInputAction inputActions, PlayerCameraController cameraController)
    {
        this.movementController = movementController;
        this.combatController = combatController;
        this.inputActions = inputActions;
        this.cameraController = cameraController;
    }

    public void ChangeState(IPlayerState newState)
    {
        if (currentState != null)
            currentState.ExitState(this);

        currentState = newState;
        currentState.EnterState(this);
    }

    public void Update()
    {
        if (currentState != null)
            currentState.UpdateState(this);
    }
}
