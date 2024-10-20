using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillsAttack : MonoBehaviour
{
    public float attackPowerA = 10f;
    public float attackPowerB = 15f;

    private BossGwak _boss;
    private int _attackCount = 0;
    private bool _isComboAttack = false;

    private void Start()
    {
        _boss = GetComponent<BossGwak>();
    }

    public void AttackA()
    {
        if (!_isComboAttack)
        {
            Debug.Log("기본 공격 A 발동");
            _attackCount++;
        }
        // TODO 데미지 처리 로직 넣기
        CheckComboAttack();
    }
    
    public void AttackB()
    {
        if (!_isComboAttack)
        {
            Debug.Log("기본 공격 A 발동");
            _attackCount++;
        }
        // TODO 데미지 처리 로직 널기
        CheckComboAttack();
    }

    private void CheckComboAttack()
    {
        if (_attackCount >= 3)
        {
            ThreeComboAttack();
        }
    }

    private void ThreeComboAttack()
    {
        Debug.Log("3연격 발동 A->B->A");
        _isComboAttack = true;
        _attackCount = 0;

        StartCoroutine(ComboCoroutine());
    }

    private IEnumerator ComboCoroutine()
    {
        AttackA();
        yield return new WaitForSeconds(2f); // 첫 A 이후 2초 대기

        AttackB();
        yield return new WaitForSeconds(3f); // B 이후 3초 대기

        AttackA();
        yield return new WaitForSeconds(0.5f); // 마지막 A 이후 0.5초 대기

        ResetComboAttack();
    }

    private void ResetComboAttack()
    {
        _isComboAttack = false;
    }
}
