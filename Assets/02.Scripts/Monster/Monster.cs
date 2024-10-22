using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private MonsterAbility monsterAbility;
    private MonsterFSM _monsterFsm;
    [SerializeField] private MonsterFSM.MonsterState _monsterState;
    void Start()
    {
        _monsterFsm = GetComponent<MonsterFSM>();
        _monsterState = MonsterFSM.MonsterState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_monsterState)
        {
            case MonsterFSM.MonsterState.Idle:
                break;
            case MonsterFSM.MonsterState.Move:
                break;
            case MonsterFSM.MonsterState.Attack:
                break;
            case MonsterFSM.MonsterState.Damage:
                break;
            case MonsterFSM.MonsterState.Die:
                break;
        }        
    }
}
