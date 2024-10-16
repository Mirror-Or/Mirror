using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 전투를 담당하는 클래스
/// </summary>
public class PlayerCombatController
{
    private PlayerStatus _playerStatus;
    private PlayerAnimationController _playerAnimationController;

    public PlayerCombatController(PlayerAnimationController animationController, PlayerStatus playerStatus)
    {
        _playerAnimationController = animationController;
        _playerStatus = playerStatus;
    }

    /// <summary>
    /// 플레이어 공격을 수행하는 함수
    /// </summary>
    /// <param name="playerPosition">플레이어 포지션</param>
    /// <param name="enemyLayer">적 Layer</param>
    public void PerformAttack(Vector3 playerPosition, LayerMask enemyLayer)
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerPosition, _playerStatus.CurrentAttackRange, enemyLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null && hitCollider.TryGetComponent<IDamage>(out var target))
            {
                target.TakeDamage((int)_playerStatus.CurrentAttackDamage);
                Debug.Log($"공격 성공: {hitCollider.name}");
            }
        }

        // 공격 애니메이션 실행
        _playerAnimationController.TriggerAnimation(AnimatorParameters.IsAttacking);
    }

    // 추후 방어 등 기능 추가
}
