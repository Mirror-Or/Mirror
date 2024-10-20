using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGwak : MonoBehaviour
{
    [SerializeField]
    private float health = 400f;
    [SerializeField]
    private float moveSpeed = 100f;
    [SerializeField]
    private float rotationSpeed = 5f;
    [SerializeField]
    private float cooldownTime = 1f;
    
    [SerializeField]
    private int _attackCount = 0;
    private float _attackCooldown = 0f;
    private int _attackRange = 200;
    private Transform _player;
    private BossSkillsAttack _bossAttack;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _bossAttack = GetComponent<BossSkillsAttack>();
    }

    private void Update()
    {
        // 플레이어 체력이 120 미만일 때만 1페이즈 로직 실행
        if (!(health > 120)) return;
        
        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
        }

        GwakBossLogic();
    }

    private void GwakBossLogic()
    {
        var playerDistance = Vector3.Distance(transform.position, _player.position);

        if (playerDistance <= _attackRange)
        {
            // 쿨타임이 없을 때만 공격
            if (!(_attackCooldown <= 0)) return;
            
            // 보스와 플레이어 간의 각도를 계산
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            
            // 120도 넘어가면
            if (angle > 120f)
            {
                Quaternion lookToPlayer = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookToPlayer, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // 보스 기준 플레이어가 왼쪽이면 A, 오른쪽이면 B
                if (_player.position.x < transform.position.x)
                {
                    _bossAttack.AttackA();
                }
                else
                {
                    _bossAttack.AttackB();
                }

                // 공격 후 쿨타임 설정
                _attackCooldown = cooldownTime;
            }
        }
        else
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Debug.Log("플레이어 추격중" + _player.position.x);
        // 플레이어 바라보기
        Vector3 direction = (_player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        
        // 이동
        transform.position += direction * (moveSpeed * Time.deltaTime);
    }
}