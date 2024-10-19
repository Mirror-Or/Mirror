using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    private PlayerFSM _playerFSM;

    // 필요한 컴포넌트
    private PlayerInputAction _playerInputAction;
    private CharacterController _characterController;
    private PlayerAnimationController _animationController;
    private PlayerMovementController _movementController;
    private PlayerCombatController _combatController;
    private PlayerStatus _playerStatus;

    [SerializeField] private LayerMask _enemyLayer;      // 적의 Layer
    [SerializeField] private LayerMask _groundLayers;    // 플레이어가 이동할 수 있는 Ground의 Layer

    [Header("Animation Settings")]
    [SerializeField] private Animator _FPSAnimator;     // 애니메이터 컴포넌트
    [SerializeField] private Animator _3stAnimator;     // 애니메이터 컴포넌트


    // 카메라 관련
    private Camera _playerCamera;                               // 플레이어 카메라
    private PlayerCameraController _cameraController;           // 플레이어 카메라 컨트롤러
    private Transform _playerChestTR;                           // 플레이어의 머리 위치
    // 케릭터 관련
    private Vector3 _characterRotationY = Vector3.zero;     // 캐싱된 회전 벡터

    private void Awake()
    {
        // 필수 컴포넌트 가져오기
        _characterController = GetComponent<CharacterController>();
        _animationController = new(_FPSAnimator, _3stAnimator);
        _playerStatus = GameManager.playerManager.GetPlayerStatus();
        _playerCamera = Camera.main;
        if (_FPSAnimator != null) _playerChestTR = _FPSAnimator.GetBoneTransform(HumanBodyBones.UpperChest);

        // 클래스 초기화
        _playerInputAction = GameManager.inputManager.GetInputActionStrategy("Player") as PlayerInputAction;
        _movementController = new(_characterController, _animationController);
        _combatController = new(_animationController, _playerStatus);
        _cameraController = new(_playerCamera.gameObject.transform);

        // FSM 초기화
        _playerFSM = new PlayerFSM(_movementController, _combatController, _playerInputAction);
        _playerFSM.ChangeState<IdleState>(); // 초기 상태는 Idle로 설정
    }

    void Update()
    {
        // 매 프레임 FSM 업데이트
        _playerFSM.Update();
    }

    /// <summary>
    /// 플레이어 회전 처리
    /// </summary>
    private void UpdateChestBoneRotation(){
        _playerChestTR.localRotation = Quaternion.Euler(0.0f, 0.0f, -_playerCamera.transform.localRotation.eulerAngles.x);
    }

    private void LateUpdate()
    {
        // 캐릭터 좌우 회전 처리 (좌우 회전은 캐릭터 회전)
        CharacterRotation();
        
        // 카메라 상하 회전 처리
        _cameraController.UpdateCameraRotation(_playerInputAction.look.y * 0.12f);
        UpdateChestBoneRotation();

        _cameraController.UpdateCameraPosition(_playerChestTR);

    }

    private void FixedUpdate() {
        _movementController.UpdateGroundedStatus(transform.position + Vector3.down * 0.14f, _groundLayers);
    }

    /// <summary>
    /// 캐릭터 회전 처리
    /// </summary>
    private void CharacterRotation()
    {
        _characterRotationY.Set(0f, _playerInputAction.look.x * 0.12f, 0f);
        _characterController.transform.Rotate(_characterRotationY);
    }
}
