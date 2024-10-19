using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState
{
    public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Entered Idle State");
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        if (playerFSM.inputActions.move.magnitude > 0.1f)
        {
            playerFSM.ChangeState(new WalkingState());
        }
        if (playerFSM.inputActions.isSit)
        {
            Debug.Log("Sit");
            playerFSM.ChangeState(new SittingState());
        }
        if (playerFSM.inputActions.isFire)
        {
            playerFSM.ChangeState(new AttackState(playerFSM.combatController, playerFSM.movementController.CurretPosition, LayerMask.GetMask("Enemy")));
        }
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Exiting Idle State");
    }
}

