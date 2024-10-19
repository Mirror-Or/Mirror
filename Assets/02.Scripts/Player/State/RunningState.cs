using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// RunningState 클래스 (뛰는 상태)
public class RunningState : IPlayerState
{
    public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Running 상태 시작");
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        playerFSM.movementController.HandleMovement(playerFSM.inputActions.move, true); // 달리기

        // 상태 전환
        if (!playerFSM.inputActions.sprint)
        {
            playerFSM.ChangeState<WalkingState>();
            return;
        }
        if (playerFSM.inputActions.jump)
        {
            playerFSM.ChangeState<JumpingState>();
            return;
        }
        if (playerFSM.inputActions.isFire)
        {
            playerFSM.ChangeState<AttackState>(playerFSM.combatController, playerFSM.movementController.CurretPosition, LayerMask.GetMask("Enemy"));
            return;
        }
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Running 상태 종료");
    }
}