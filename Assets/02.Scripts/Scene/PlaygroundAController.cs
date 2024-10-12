using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundAController : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    void Start()
    {
        // 현재 씬에서 게임 매니저 오브젝트가 없다면 생성 진행
        GameObject gameManager = GameObject.Find("GameManager");
        if(gameManager == null){
            gameManager = Instantiate(Resources.Load("Prefabs/GameManager") as GameObject);
            gameManager.GetComponent<GameManager>().Initialize(SceneConstants.PlaygroundA);
        }


        Vector3 spawnPoint = new(0, 4.0f, 0);
        GameManager.playerManager.SpawnPlayer(spawnPoint);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
