using System.Collections.Generic;
using UnityEngine;

public class AttackState : IPlayerState
{
    private PlayerCombatController _combatController;
    private Vector3 _playerPosition;
    private LayerMask _enemyLayer;

    public AttackState(PlayerCombatController combatController, Vector3 playerPosition, LayerMask enemyLayer)
    {
        _combatController = combatController;
        _playerPosition = playerPosition;
        _enemyLayer = enemyLayer;
    }

    public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Entered Attack State");
        _combatController.PerformAttack(_playerPosition, _enemyLayer);
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        playerFSM.ChangeState(new IdleState()); // 공격 후 Idle 상태로 전환
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Exiting Attack State");
    }
}
