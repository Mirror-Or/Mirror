using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterManager :  IManager
{
    public List<GeneratorMonsterInfo> generatorMonsterInfos; // 몬스터 정보가 담길 리스트
    // private List<GameObject> _foundMonsterList;              // 상위 프리팹이 담길 리스트
    public List<GameObject> settingMonsterList;              // 몬스터의 정보를 수정하고 담을 리스트 

    private MonsterFSM _monsterFsm;                          // 몬스터 FSM 컴포넌트
    
    public enum MonsterType
    {
        Student = 0,
        Mirror = 1
    }
    public void SettingListAdd()                            // 몬스터의 정보를 넘기는 함수
    {
        for (int i = 0; i < generatorMonsterInfos.Count; i++)
        {
            settingMonsterList.Add(ObjectPoolManager.Instance.GetPoolObject(generatorMonsterInfos[i].monsterType.ToString()));
        }
        for (int i = 0; i < generatorMonsterInfos.Count; i++)
        {
            // int chileIndex = (int)generatorMonsterInfos[i].monsterType;     //타입이 뭔지 가져오기
            // settingMonsterList.Add(_foundMonsterList[i].gameObject.transform.GetChild(chileIndex).gameObject);// 타입에 따라 프리팹 활성화
            settingMonsterList[i].gameObject.SetActive(true);
            _monsterFsm = settingMonsterList[i].GetComponent<MonsterFSM>();
            _monsterFsm.IsMovingMonster = generatorMonsterInfos[i].isMoving;    // 움직일지 여부 전달
            _monsterFsm.MovePositionGroup = generatorMonsterInfos[i].movePositionGroup;// 움직일 경로 전달
            _monsterFsm.StartPosition = generatorMonsterInfos[i].startPosition.position;
        }
    }

    public void Initialize(string sceneName)
    {
        // if(sceneName == SceneConstants.PlaygroundA) // 현재 씬이 PlaygroundA라면
        // {
        settingMonsterList = new List<GameObject>();
        //}
    }

    public void Generator(List<GeneratorMonsterInfo> generatorMonster) // 몬스터 인포 정보가 넘어와 리스트에 담는 함수
    {
        generatorMonsterInfos = generatorMonster;
        SettingListAdd();
    }
}
