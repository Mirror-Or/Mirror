using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// JumpingState 클래스 (점프 상태)
public class JumpingState : IPlayerState
{
    public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Jumping 상태 시작");
        playerFSM.movementController.HandleJump(true);  // 점프 한 번만 실행
    }

    public void UpdateState(PlayerFSM playerFSM)
    { 
        // 착지 상태로 전환
        if (playerFSM.movementController.IsGrounded)
        {
            playerFSM.ChangeState<IdleState>();  // 착지하면 Idle 상태로 전환
        }
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Jumping 상태 종료");
        playerFSM.movementController.HandleJump(false);  // 점프 한 번만 실행
    }
}