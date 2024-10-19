using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState
{
    public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Idle 상태 시작");
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        if (playerFSM.inputActions.jump)
        {
            playerFSM.ChangeState<JumpingState>();
            return;
        }
        if (playerFSM.inputActions.isSit)
        {
            playerFSM.ChangeState<SittingState>();
            return;
        }
        if (playerFSM.inputActions.isFire)
        {
            playerFSM.ChangeState<AttackState>(playerFSM.combatController, playerFSM.movementController.CurretPosition, LayerMask.GetMask("Enemy"));
            return;
        }
        if (playerFSM.inputActions.move != Vector2.zero)
        {
            playerFSM.ChangeState<WalkingState>();
            return;
        }

        playerFSM.movementController.HandleMovement(playerFSM.inputActions.move, false);
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Idle 상태 종료");
    }
}

