using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SittingState : IPlayerState
{
    public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Sitting 상태 시작");
        playerFSM.movementController.HandleSit(true); // 앉기 시작
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        playerFSM.movementController.HandleMovement(playerFSM.inputActions.move, false);
        
        if (playerFSM.inputActions.isSit == false)
        {
            playerFSM.ChangeState<IdleState>();
        }
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Sitting 상태 종료");
        playerFSM.movementController.HandleSit(false); // 앉기 해제
    }
}