using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utils;

/// <summary>
/// 플레이어의 키 입력에 따른 비즈니스 로직을 담당하는 클래스
/// </summary>
public class PlayerStateController : MonoBehaviour
{
    private PlayerStatus _playerStatus;             // PlayerStatus를 참조
    private float _rotationSmoothTime = 0.12f;      // 회전 부드러움 시간

    // 공격 세팅
    private float _attackTimeoutDelta = 0.0f;     // 공격 타임아웃 델타

    [Header("Animation Settings")]
    [SerializeField] private Animator _FPSAnimator;     // 애니메이터 컴포넌트
    [SerializeField] private Animator _3stAnimator;     // 애니메이터 컴포넌트

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

    // 케릭터 관련
    private Vector3 _characterRotationY = Vector3.zero;     // 캐싱된 회전 벡터

    private void Awake()
    {
        InitializeAnimators();
        InitializeComponents();
        InitializeControllers();
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
    }

    private void LateUpdate()
    {
        // 캐릭터 좌우 회전 처리 (좌우 회전은 캐릭터 회전)
        CharacterRotation();
        
        // 카메라 상하 회전 처리
        _cameraController.UpdateCameraRotation(_inputActions.look.y * _rotationSmoothTime);
        UpdateChestBoneRotation();

        _cameraController.UpdateCameraPosition(_playerChestTR);

    }

    private void FixedUpdate() {
        _movementController.UpdateGroundedStatus(transform.position + Vector3.down * 0.14f, _groundLayers);
    }

    #region Initialization
    /// <summary>
    /// 애니메이터 초기화
    /// </summary>
    private void InitializeAnimators()
    {
        if (_FPSAnimator != null) _playerChestTR = _FPSAnimator.GetBoneTransform(HumanBodyBones.UpperChest);
    }

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void InitializeComponents()
    {
        _playerCamera = Camera.main;
        _playerStatus = GameManager.playerManager.GetPlayerStatus();
        _characterController = GetComponent<CharacterController>();
        _inputActions = GameManager.inputManager.GetInputActionStrategy("Player") as PlayerInputAction;
    }

    /// <summary>
    /// 컨트롤러 초기화
    /// </summary>
    private void InitializeControllers()
    {
        _cameraController = new(_playerCamera.gameObject.transform);
        _objectDetectedController = new(_playerCamera, _maxDistance);
        _animationController = new(_FPSAnimator, _3stAnimator);
        _movementController = new(_characterController, _animationController);
        _interactionController = new(_objectDetectedController);
        _combatController = new(_animationController, _playerStatus);
    }
    #endregion

    #region Rotation
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
        _characterRotationY.Set(0f, _inputActions.look.x * _rotationSmoothTime, 0f);
        _characterController.transform.Rotate(_characterRotationY);
    }
    #endregion

    /// <summary>
    /// 상호작용 오브젝트 처리
    /// </summary>
    private void InteractionObject()
    {
        // 키를 입력 받은 경우 
        if (_inputActions.isInteractable)
        {
            _interactionController.HandleInteraction();
            _inputActions.isInteractable = false;
        }
    }

    // 추후 UI 관련 로직은 별도의 클래스로 분리할 예정
    #region 아이테 사용 및 전달
    private void TransferItem()
    {

    }

    private void ShowInventory()
    {

    }

    /// <summary>
    /// 아이템 사용
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
        if (_inputActions.isFire && _attackTimeoutDelta <= 0.0f)
        {
            _combatController.PerformAttack(transform.position, LayerMask.GetMask("Enemy"));
            StartCoroutine(AttackDelay());
        }
    }

    private IEnumerator AttackDelay()
    {
        _attackTimeoutDelta = PlayerBasicSettings.attackDelay;
        yield return new WaitForSeconds(_attackTimeoutDelta);
        _attackTimeoutDelta = 0.0f;
    }

}