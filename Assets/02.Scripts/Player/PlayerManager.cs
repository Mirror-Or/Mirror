using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager :  IManager, IDamage
{
	private GameObject _player; 		// 플레이어 오브젝트
	private Vector3 _playerSpawnPoint; // 플레이어 스폰 위치

    private PlayerStatus playerStatus; // 플레이어 상태 클래스
	public Transform playerTransform;  // 플레이어 위치

	private AudioClip[] hitSound;    // 피격 효과음
    private AudioClip deathSound;    // 사망 효과음

	private void Awake(){
		// 사운드 세팅 필요
	}

	/// <summary>
	/// 특정 위치에 플레이어 생성
	/// </summary>
	/// <param name="spawnPoint">스폰 위치</param>
	public void SpawnPlayer(Vector3 spawnPoint){
		if(_player == null){
			_player = Object.Instantiate(GameManager.resourceManager.LoadResource<GameObject>("Prefabs/Player/MainPlayer"), spawnPoint,Quaternion.identity);
		}else{
			_player.transform.position = spawnPoint;
		}
	}

	public void SpawnPlayer(){
		SpawnPlayer(_playerSpawnPoint);
	}

	public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        _playerSpawnPoint = newSpawnPoint;
    }

	public GameObject GetPlayer()
    {
        return _player;
    }

	public void Initialize(string sceneName){
		if(sceneName == SceneConstants.PlaygroundA)
		{
			// if(playerStatus == null) playerStatus = GetComponent<PlayerStatus>();

			// // 추후에는 해당 씬의 Player를 찾아서 할당해야 함
			// playerTransform = GameObject.FindGameObjectsWithTag("Player")[0].transform;
		}
	}

	public PlayerStatus GetPlayerStatus(){
		playerStatus ??= new();

		return playerStatus;
	}	

	public void TakeDamage(int hitPower)
    {
        if(playerStatus.CurrentHealth <= 0) return;  // 이미 사망한 경우 데미지를 받지 않음

        playerStatus.AdjustStatus(StatusType.Health, -hitPower);

        // 피격 효과음 재생
        // if(playerStatus.CurrentHealth > 0){
		// 	GameManager.audioManager.PlaySoundEffect(hitSound[Random.Range(0, hitSound.Length)], playerTransform.position, 1.0f);

        // }else{
		// 	GameManager.audioManager.PlaySoundEffect(deathSound, playerTransform.position, 1.0f);
        // }

        Debug.Log($"현재 체력 : {playerStatus.CurrentHealth}");
    }

}
