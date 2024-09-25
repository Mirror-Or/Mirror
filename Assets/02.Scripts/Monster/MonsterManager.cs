using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : Singleton<MonsterManager>, IManager
{
    public List<GeneratorMonsterInfo> generatorMonsterInfos;
    public enum MonsterType
    {
        Student = 0,
        Mirror = 1
    }
    public void Initialize(string sceneName)
    {
        if(sceneName == SceneConstants.PlaygroundA) // 현재 씬이 PlaygroundA라면
        {
            
        }
    }
}
