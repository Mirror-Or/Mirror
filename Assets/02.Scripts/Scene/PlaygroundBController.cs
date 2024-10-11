using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundBController : MonoBehaviour
{
    private AudioManager _audioManager;

    [SerializeField] private GameObject _player;

    void Start()
    {
        _audioManager = GameManager.audioManager;

        Vector3 spawnPoint = new(0, 0, 0);
        GameManager.playerManager.SpawnPlayer(spawnPoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
