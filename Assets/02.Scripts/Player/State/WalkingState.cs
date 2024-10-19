
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : IPlayerState
{
	public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Entered Walking State");
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        playerFSM.movementController.HandleMovement(playerFSM.inputActions.move, false); 

        // 상태 전환 조건
        if (playerFSM.inputActions.move.magnitude < 0.1f)
        {
            playerFSM.ChangeState(new IdleState());
        }
        if (playerFSM.inputActions.sprint)
        {
            playerFSM.ChangeState(new RunningState());
        }
        if (playerFSM.inputActions.jump)
        {
            playerFSM.ChangeState(new JumpingState());
        }
        if (playerFSM.inputActions.isFire)
        {
            playerFSM.ChangeState(new AttackState(playerFSM.combatController, playerFSM.movementController.CurretPosition, LayerMask.GetMask("Enemy")));
        }
        if (playerFSM.inputActions.isSit)
        {
            playerFSM.ChangeState(new SittingState());
        }
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Exiting Walking State");
    }
}
