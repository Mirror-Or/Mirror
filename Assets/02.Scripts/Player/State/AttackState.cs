using System.Collections.Generic;
using UnityEngine;

public class AttackState : IPlayerState
{
    private PlayerCombatController _combatController;
    private Vector3 _playerPosition;
    private int _enemyLayer;

    // 공격 시간 관련 변수
    private float _attackDuration = 1.0f;  // 공격이 끝날 때까지 기다릴 시간
    private float _attackTimer;

    public AttackState(PlayerCombatController combatController, Vector3 playerPosition, int enemyLayer)
    {
        _combatController = combatController;
        _playerPosition = playerPosition;
        _enemyLayer = enemyLayer;
    }

    public void EnterState(PlayerFSM playerFSM)
    {
        Debug.Log("Attack 상태 시작");
        _combatController.PerformAttack(_playerPosition, _enemyLayer);
         _attackTimer = _attackDuration;  // 상태가 시작되면 타이머 초기화
    }

    public void UpdateState(PlayerFSM playerFSM)
    {
        _attackTimer -= Time.deltaTime;  // 공격 타이머 감소

        if(_attackTimer <= 0.0f)
        {
            playerFSM.ChangeState<IdleState>();         // 공격 후 Idle 상태로 전환
        }
    }

    public void ExitState(PlayerFSM playerFSM)
    {
        Debug.Log("Attack 상태 종료");
    }
}
