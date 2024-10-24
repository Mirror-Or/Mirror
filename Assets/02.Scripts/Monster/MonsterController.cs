using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public enum MonsterState
    {
        Idle,
        Move,
        Attack,
        Damage,
        Die
    }

    [SerializeField] private MonsterAbility monsterAbility;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
