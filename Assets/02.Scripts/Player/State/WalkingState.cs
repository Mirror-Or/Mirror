
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : IPlayerState
{
	public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Walking 상태 시작");
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        playerFSM.movementController.HandleMovement(playerFSM.inputActions.move, false); 

        // 상태 전환 조건
        if (playerFSM.inputActions.move == Vector2.zero)
        {
            playerFSM.ChangeState<IdleState>();
            return;
        }
        if (playerFSM.inputActions.sprint)
        {
            playerFSM.ChangeState<RunningState>();
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
        if (playerFSM.inputActions.isSit)
        {
            playerFSM.ChangeState<SittingState>();
            return;
        }
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Walking 상태 종료");
    }
}
