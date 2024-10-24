using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour
{
    
}
public class MonsterIdle : MonoBehaviour, IState<MonsterController>
{
    
    private MonsterController _monsterController;

    public void OperateEnter(MonsterController sender)
    {
        _monsterController = sender;
    }

    public void OperateUpdate(MonsterController sender)
    {
        
    }

    public void OperateExit(MonsterController sender)
    {
        
    }
}
public class MonsterMove : MonoBehaviour, IState<MonsterController>
{
    
    private MonsterController _monsterController;

    public void OperateEnter(MonsterController sender)
    {
        _monsterController = sender;
    }

    public void OperateUpdate(MonsterController sender)
    {
        
    }

    public void OperateExit(MonsterController sender)
    {
        
    }
}

public class MonsterAttack : MonoBehaviour, IState<MonsterController>
{
    
    private MonsterController _monsterController;

    public void OperateEnter(MonsterController sender)
    {
        _monsterController = sender;
    }

    public void OperateUpdate(MonsterController sender)
    {
        
    }

    public void OperateExit(MonsterController sender)
    {
        
    }
}

public class MonsterDamage : MonoBehaviour, IState<MonsterController>
{
    
    private MonsterController _monsterController;

    public void OperateEnter(MonsterController sender)
    {
        _monsterController = sender;
    }

    public void OperateUpdate(MonsterController sender)
    {
        
    }

    public void OperateExit(MonsterController sender)
    {
        
    }
}

public class MonsterDie : MonoBehaviour, IState<MonsterController>
{
    
    private MonsterController _monsterController;

    public void OperateEnter(MonsterController sender)
    {
        _monsterController = sender;
    }

    public void OperateUpdate(MonsterController sender)
    {
        
    }

    public void OperateExit(MonsterController sender)
    {
        
    }
}
