using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utils;

/// <summary>
/// 플레이어의 상태(이동, 점프, 달리기등)에 대한 로직을 담당하는 클래스
/// </summary>
public class PlayerStateController : MonoBehaviour
{
    private PlayerStatus _playerStatus;             // PlayerStatus를 참조
    private float _rotationSmoothTime = 0.12f;      // 회전 부드러움 시간

    // 공격 세팅
    private float _attackTimeoutDelta = 0.0f;     // 공격 타임아웃 델타


    [Header("Animation Settings")]
    private bool _hasFPSAnimator;      // 애니메이터가 있는지 여부
    private bool _has3stAnimator;      // 애니메이터가 있는지 여부
    [SerializeField] private Animator _FPSAnimator;     // 애니메이터 컴포넌트
    [SerializeField] private Animator _3stAnimator;     // 애니메이터 컴포넌트


    private bool _isQuickSlotCurrentlyVisible  = false;         // 현재 퀵슬롯 활성화 여부

    private CharacterController _characterController;           //  캐릭터 컨트롤러
    private PlayerInputAction _inputActions;                    // 플레이어 입력 액션

    // 이동 관련
    private PlayerMovementController _movementController;       // 플레이어 이동 컨트롤러
    [SerializeField] private LayerMask _groundLayers;           // 플레이어가 이동할 수 있는 Ground의 Layer

    private PlayerAnimationController _animationController;      // 플레이어 애니메이션 컨트롤러
    private PlayerInteractionController _interactionController;  // 플레이어 상호작용 컨트롤러
    private PlayerCombatController _combatController;            // 플레이어 전투 컨트롤러

    // 카메라 관련
    private Camera _playerCamera;                               // 플레이어 카메라
    private PlayerCameraController _cameraController;           // 플레이어 카메라 컨트롤러
    private Transform _playerChestTR;                           // 플레이어의 머리 위치
    
    // 오브젝트 감지 관련
    private PlayerObjectDetectedController _objectDetectedController;   // 플레이어 오브젝트 감지 컨트롤러
    private readonly float _maxDistance = 10.0f;                        // 오브젝트 감지 최대 거리

    private void Awake()
    {
        _hasFPSAnimator = _FPSAnimator != null;
        _has3stAnimator = _3stAnimator != null;
        Debug.Log($"FPS Animator: {_hasFPSAnimator} / 3st Animator: {_has3stAnimator}");

        // animator가 있는 경우, 머리 위치를 가져옴
        if(_hasFPSAnimator) _playerChestTR = _FPSAnimator.GetBoneTransform(HumanBodyBones.UpperChest);
        
        // 컴포넌트 및 클래스 참조
        _playerCamera = Camera.main;
        _playerStatus = GameManager.playerManager.GetPlayerStatus();
        _characterController = GetComponent<CharacterController>();
        _inputActions = GameManager.inputManager.GetInputActionStrategy("Player") as PlayerInputAction;

        // 컨트롤러 참조
        _cameraController = new(_playerCamera.gameObject.transform);
        _objectDetectedController = new(_playerCamera,_maxDistance);
        _animationController = new(_FPSAnimator, _3stAnimator);
        _movementController = new(_characterController,_animationController);
        _interactionController = new(_objectDetectedController);
        _combatController = new(_animationController, _playerStatus);

    }

    private void Update()
    {
        // 입력에 따라 이동 명령 전달
        _movementController.HandleMovement(_inputActions.move, _inputActions.sprint);
        _objectDetectedController.DetectObject();
        

        if(_inputActions.jump){
            _movementController.HandleJump(true);
        }else{
            _movementController.HandleJump(false);
        }

        if(_inputActions.isSit){
            _movementController.HandleSit(true);
        }else{
            _movementController.HandleSit(false);
        }

        OnAttack();             // 공격 처리
        InteractionObject();    // 상호작용 오브젝트 처리

        // UseItem();
        // TransferItem();
        // ShowInventory();
        // ShowQuickSlot();
        // SetSelectItem();

        _attackTimeoutDelta += Time.deltaTime;  // 공격 타임아웃 델타 증가
    }

    private void LateUpdate()
    {
        // 캐릭터 좌우 회전 처리 (좌우 회전은 캐릭터 회전)
        CharacterRotation();
        
        // 카메라 상하 회전 처리
        _cameraController.UpdateCameraRotation(_inputActions.look.y * _rotationSmoothTime);
        UpdateChestBoneRotation();

        _playerCamera.transform.position = _playerChestTR.transform.position + _playerChestTR.transform.right * -0.2f;

    }

    private void FixedUpdate() {
        _movementController.UpdateGroundedStatus(transform.position + Vector3.down * 0.14f, _groundLayers);
    }
    /// <summary>
    /// 플레이어 회전 처리
    /// </summary>
    private void UpdateChestBoneRotation(){
        _playerChestTR.localRotation = Quaternion.Euler(0.0f, 0.0f, -_playerCamera.transform.localRotation.eulerAngles.x);
    }

    /// <summary>
    /// 캐릭터 회전 처리
    /// </summary>
    private void CharacterRotation()
    {
        Vector3 _characterRotationY = new Vector3(0f, _inputActions.look.x, 0f) * _rotationSmoothTime;
        _characterController.transform.Rotate(_characterRotationY);
    }


    /// <summary>
    /// 상호작용 오브젝트 처리
    /// </summary>
    private void InteractionObject()
    {
        // 키를 입력 받은 경우 
        if (_inputActions.isInteractable)
        {
            _interactionController.HandleInteraction();
        }
    }

    #region 아이테 사용 및 전달
    private void TransferItem()
    {

    }

    private void ShowInventory()
    {

    }

    /// <summary>
    /// 아이템 사용
    /// @Todo: 추후 로직을 다른 곳으로 이동 시킬지 고민 필요
    /// </summary>
    private void UseItem()
    {
        if (_inputActions.isUseItem)
        {
            Debug.Log("아이템 사용");
            _inputActions.isUseItem = false;
        }
    }

    private void SetSelectItem(){
        if(_inputActions.isChoiceQuickSlot){
            Debug.Log("퀵슬롯 선택");
        }
    }

    private void ShowQuickSlot(){

    }
    #endregion

    // 공격 처리
    private void OnAttack()
    {
        if(_inputActions.isFire)
        {
            Debug.Log($"{_attackTimeoutDelta} / {PlayerBasicSettings.attackDelay}");

            if(_attackTimeoutDelta > PlayerBasicSettings.attackDelay){
                _combatController.PerformAttack(transform.position, LayerMask.GetMask("Enemy"));
                _attackTimeoutDelta = 0.0f; // 공격 타임아웃 초기화
            }else{
                Debug.Log("공격 딜레이 중");
            }
        }
    }

}


