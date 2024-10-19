using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterManager :  IManager
{
    public List<GeneratorMonsterInfo> GeneratorMonsterInfos{ get; set; } // 몬스터 정보가 담길 리스트
    // private List<GameObject> _foundMonsterList;              // 상위 프리팹이 담길 리스트
    public List<GameObject> SettingMonsterList = new List<GameObject>();              // 몬스터의 정보를 수정하고 담을 리스트 

    private MonsterFSM _monsterFsm;                          // 몬스터 FSM 컴포넌트
    
    public enum MonsterType
    {
        Student = 0,
        Mirror = 1
    }
    public void SettingListAdd()                            // 몬스터의 정보를 넘기는 함수
    {
        for (int i = 0; i < GeneratorMonsterInfos.Count; i++)
        {
            // 아직 게임매니저에 오브젝트풀매니저가 등록되지 않아 인스턴스로 사용하였습니다.
            SettingMonsterList.Add(ObjectPoolManager.Instance.GetPoolObject(GeneratorMonsterInfos[i].monsterType.ToString()));
        }
        for (int i = 0; i < GeneratorMonsterInfos.Count; i++)
        { 
            SettingMonsterList[i].gameObject.SetActive(true);
            _monsterFsm = SettingMonsterList[i].GetComponent<MonsterFSM>();
            _monsterFsm.IsMovingMonster = GeneratorMonsterInfos[i].isMoving;    // 움직일지 여부 전달
            _monsterFsm.MovePositionGroup = GeneratorMonsterInfos[i].movePositionGroup;// 움직일 경로 전달
            _monsterFsm.StartPosition = GeneratorMonsterInfos[i].startPosition.position;
        }
    }

    public void Initialize(string sceneName)
    {
        // if(sceneName == SceneConstants.PlaygroundA) // 현재 씬이 PlaygroundA라면
        // {
        SettingMonsterList = new List<GameObject>();
        //}
    }

    public void Generator(List<GeneratorMonsterInfo> generatorMonster) // 몬스터 인포 정보가 넘어와 리스트에 담는 함수
    {
        GeneratorMonsterInfos = generatorMonster;
        SettingListAdd();
    }
}
