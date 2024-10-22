using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MonsterAbility",menuName = "MstAbility",order = 0)]
public class MonsterAbility : ScriptableObject
{
    public float MonsterHP;
    public float MoveSpeed = 8f;       // 학생(몬스터)의 이동속도
    public float AttackDistance = 2f;      // 학생(몬스터)의 공격범위
    public float AttackDelay = 2f;     // 학생(몬스터)의 공격 딜레이
    public float AttackPower = 5f;
}
