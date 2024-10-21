using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldMonster : MonoBehaviour
{
    //   기본 상태값
    public Vector3 StartPosition;     // 학생(몬스터)의 시작 위치
    [SerializeField] protected float monsterHp = 20f;      // 학생(몬스터)의 체력
    [SerializeField] protected float attackDistance = 2f;      // 학생(몬스터)의 공격범위
    [SerializeField] protected float moveSpeed = 8f;       // 학생(몬스터)의 이동속도
    [SerializeField] protected float attackDelay = 2f;     // 학생(몬스터)의 공격 딜레이
    [SerializeField] protected float attackPower = 5f;     // 학생(몬스터)의 공격력
}
