using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SittingState : IPlayerState
{
    public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Entered Sitting State");
        playerFSM.movementController.HandleSit(true); // 앉기 시작
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        if (playerFSM.inputActions.isSit == false)
        {
            playerFSM.ChangeState(new WalkingState());
        }
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Exiting Sitting State");
        playerFSM.movementController.HandleSit(false); // 앉기 해제
    }
}