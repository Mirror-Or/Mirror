using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFSM : MonoBehaviour
{
    public MonsterState monsterState;
    public enum MonsterState
    {
        Idle,
        Move,
        Attack,
        Damage,
        Die
    }

    public void ChangeState(MonsterState nowState,MonsterState nextState)
    {
        // 스태이트 바꿔주는 코드
    }
}
