using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// JumpingState 클래스 (점프 상태)
public class JumpingState : IPlayerState
{
    public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Entered Jumping State");
        playerFSM.movementController.HandleJump(true); // 점프 시작
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        // 착지 상태로 전환
        if (playerFSM.movementController.IsGrounded)
        {
            playerFSM.ChangeState(new WalkingState());
        }

        // 공격 상태로 전환
        if (playerFSM.inputActions.isFire)
        {
            playerFSM.ChangeState(new AttackState(playerFSM.combatController, playerFSM.movementController.CurretPosition, LayerMask.GetMask("Enemy")));
        }
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Exiting Jumping State");
    }
}