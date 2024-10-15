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

    [Header("Attack Settings")]
    private float _attackTimeoutDelta = 0.0f;     // 공격 타임아웃 델타

    [SerializeField] private LayerMask _groundLayers; // 플레이어가 이동할 수 있는 Ground의 Layer

    [Header("Animation Settings")]
    private bool _hasFPSAnimator;      // 애니메이터가 있는지 여부
    private bool _has3stAnimator;      // 애니메이터가 있는지 여부
    [SerializeField] private Animator _FPSAnimator;     // 애니메이터 컴포넌트
    [SerializeField] private Animator _3stAnimator;     // 애니메이터 컴포넌트

    // animation IDs
    private int _animIDAttack;
    private int _animIDSit;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;           // 오디오 소스
    [SerializeField] private AudioClip attackAudioClip;         // 공격 사운드


    [SerializeField] private PlayerInputAction _inputActions;   // 플레이어 입력 액션
    private CharacterController _characterController;           //  캐릭터 컨트롤러

    // camera settings
    [SerializeField] private GameObject _cinemachineCamera;     // 카메라
    private Transform _playerChestTR;                           // 플레이어의 머리 위치
    private float _cinemachineTargetPitch = 0.0f;               // 현재 카메라 회전 각도
    public float topClamp = 70.0f;                              // 카메라 상단 회전 제한 각도
    public float bottomClamp = -30.0f;                          // 카메라 하단 회전 제한 각도
    public float cameraAngleOverride = 0.0f;                    // 카메라 각도 오버라이드

    private bool _isQuickSlotCurrentlyVisible  = false;         // 현재 퀵슬롯 활성화 여부
    private bool _isSitVisible = false;                         // 현재 앉기 상태인지 여부

    private PlayerMovementController _movementController;       // 플레이어 이동 컨트롤러
    private PlayerAnimationController _animationController;     // 플레이어 애니메이션 컨트롤러

    private void Start()
    {
        _hasFPSAnimator = _FPSAnimator != null;
        _has3stAnimator = _3stAnimator != null;
        Debug.Log($"FPS Animator: {_hasFPSAnimator} / 3st Animator: {_has3stAnimator}");

        _playerStatus = GameManager.playerManager.GetPlayerStatus();
        Debug.Log($"Player Status: {_playerStatus.CurrentHealth}");
        _characterController = GetComponent<CharacterController>();
        _inputActions = GameManager.inputManager.GetInputActionStrategy("Player") as PlayerInputAction;

        AssignAnimationIDs();

        // animator가 있는 경우, 머리 위치를 가져옴
        if (_hasFPSAnimator)
        {
            _playerChestTR = _FPSAnimator.GetBoneTransform(HumanBodyBones.UpperChest);
        }

        _animationController = new PlayerAnimationController(_FPSAnimator, _3stAnimator);
        _movementController = new PlayerMovementController(_characterController, _inputActions,_animationController);
    }

    private void Update()
    {
        _movementController.HandleMovement();

        UseItem();
        InteractionObject();
        TransferItem();
        ShowInventory();
        OnFire();
        ShowQuickSlot();

        SetSelectItem();

        _attackTimeoutDelta += Time.deltaTime;  // 공격 타임아웃 델타 증가
    }

    private void LateUpdate()
    {
        CharacterRotation();
        UpdateCameraRotation();
        UpdateChestBoneRotation();

        _cinemachineCamera.transform.position = _playerChestTR.transform.position + _playerChestTR.transform.right * -0.2f;

    }

    private void FixedUpdate() {
        _movementController.UpdateGroundedStatus(transform.position + Vector3.down * 0.14f, _groundLayers);
    }


    /// <summary>
    /// 애니메이션 ID를 할당
    /// </summary>
    private void AssignAnimationIDs()
    {
        _animIDAttack = AnimatorParameters.AnimIDAttack;
        _animIDSit = AnimatorParameters.AnimIDSit;
    }


    /// <summary>
    /// 카메라 회전 처리
    /// </summary>
    private void UpdateCameraRotation(){
        float _xRotation = _inputActions.look.y * _rotationSmoothTime;    // 상하 회전

        _cinemachineTargetPitch -= _xRotation;
        // _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);
        _cinemachineTargetPitch = MathUtil.ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        Quaternion targetRotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, 0.0f, 0.0f);
        _cinemachineCamera.transform.localRotation = targetRotation;
    }

    /// <summary>
    /// 플레이어  회전 처리
    /// </summary>
    private void UpdateChestBoneRotation(){
        _playerChestTR.localRotation = Quaternion.Euler(0.0f, 0.0f, -_cinemachineCamera.transform.localRotation.eulerAngles.x);
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

    /// <summary>
    /// 상호작용 오브젝트 처리
    /// @Todo: 아이템 외에도 상호작용 오브젝트에 대한 처리가 필요
    /// </summary>
    private void InteractionObject()
    {
        // 키를 입력 받은 경우 
        if (_inputActions.isInteractable)
        {
            GameManager gameManager = GameManager.Instance;
            PlayerCameraController cameraController = GameManager.cameraManager.GetCameraController<PlayerCameraController>("PlayerCamera");
            GameObject detetedItem = cameraController.detectedObject;
            
            // 아이템이 감지된 경우
            if (detetedItem != null)
            {
                IInventoryItem inventoryItem = detetedItem.GetComponent<IInventoryItem>();
                if(inventoryItem.IsPickable == false){
                    Debug.Log("획득할 수 없는 아이템입니다.");
                    _inputActions.isInteractable = false;
                    return;
                }

                detetedItem.SetActive(false);
                inventoryItem.IsActive = false;
                //inventoryItem.Count += 1; // 아이템 개수 증가
                
                //gameManager.playerInventory.AddItem(inventoryItem);// 플레이어 인벤토리에 아이템 추가
                gameManager.inventoryManager.AddItem(inventoryItem);
                // if(inventoryItem.UseSound != null) gameManager.itemManager.PlaySound(inventoryItem.PickSound); // 아이템 획득 사운드 재생
                audioSource.clip = inventoryItem.PickSound;
                audioSource.Play();
                
                Debug.Log("아이템 획득");
                cameraController.detectedObject = null;
            }

            _inputActions.isInteractable = false;
        }
    }

    private void TransferItem()
    {
        if (_inputActions.isTransferItem)
        {

            Debug.Log("아이템 이동");
            // @TODO: 추후 임시코드 제거 및 해당 아이템이 인벤토리에 있는지 확인하는 코드 추가
            // @TODO: 추후 TransferItem 함수를 합칠지 고민 필요

            // 임시 테스트 코드 진행
            // 인벤토리에 있는 아이템 하나를 창고로 이동
            // ItemManager.Instance.TransferItem(PlayerInventory.Instance, Storage.Instance, PlayerInventory.Instance.items[0]);
            _inputActions.isTransferItem = false;
        }
    }

    /// <summary>
    /// 아이템 이동 함수
    /// </summary>
    /// <param name="from">아이템 존재하는 위치</param>
    /// <param name="to">아이템을 이동시킬 위치</param>
    /// <param name="item">전달하고자 하는 아이템</param>
    private void TransferItem(IItemContainer from, IItemContainer to, BaseItem item){
        // @ TODO: 아이템 이동 로직 구현 아직 미완성

        if(from != null && to != null){
            from.RemoveItem(item);
            to.AddItem(item);
            Debug.Log("아이템 이동");
        }else{
            Debug.LogError("아이템 이동 실패");
        }
    }

    private void ShowInventory()
    {
        if (_inputActions.isInventoryVisible)
        {
            GameManager gameManager = GameManager.Instance;     // MainGameManager 인스턴스

            // 임시 테스트용
            InventoryManager inventoryManager = gameManager.inventoryManager;  // InventoryManager_Test 인스턴스
            UIManager uIManager = GameManager.uiManager;

            PlayerCameraController cameraController = GameManager.cameraManager.GetCameraController<PlayerCameraController>("PlayerCamera");

            // 인벤토리 UI 활성화/비활성화
            inventoryManager.OnShowInventory();
            cameraController.SetCursorState(uIManager.Inventory().gameObject.activeSelf);   // 커서 상태 설정

            _inputActions.isInventoryVisible = false;
        }
    }

    private void ShowQuickSlot(){
        // 임시 테스트용
        InventoryManager inventoryManager = GameManager.Instance.inventoryManager; // InventoryManager_Test 인스턴스
        if(_inputActions.isQuickSlotVisible){
            // Debug.Log("퀵슬롯 활성화/비활성화");

            if(!_isQuickSlotCurrentlyVisible){
                _isQuickSlotCurrentlyVisible = true;
                inventoryManager.OnShowQuickSlot();
            }

        }else{
            if(_isQuickSlotCurrentlyVisible){
                _isQuickSlotCurrentlyVisible = false;
                inventoryManager.OnShowQuickSlot();
            }
        }
    }

    private void OnFire()
    {
        if(_inputActions.isFire)
        {
            Debug.Log($"{_attackTimeoutDelta} / {PlayerBasicSettings.attackDelay}");

            if(_attackTimeoutDelta > PlayerBasicSettings.attackDelay){
                PerformAttack();
                _attackTimeoutDelta = 0.0f; // 공격 타임아웃 초기화
            }else{
                Debug.Log("공격 딜레이 중");
            }

            _inputActions.isFire = false;
        }
    }

    // 범위 내의 적의 Collider를 가져옴
    private Collider[] GetEnemiesInRange(Vector3 position, float range){
        return Physics.OverlapSphere(position, range, LayerMask.GetMask("Enemy"));
    }

    private void PerformAttack(){
        Debug.Log($"Attack Range: {_playerStatus.CurrentAttackRange} / playerPosition: {transform.position}");

        // 애니메이터 처리
        if(_hasFPSAnimator) _FPSAnimator.SetTrigger(_animIDAttack);
        if(_has3stAnimator) _3stAnimator.SetTrigger(_animIDAttack);


        // 공격 사거리 내 적을 가져옴
        Collider[] hitColliders = GetEnemiesInRange(transform.position, _playerStatus.CurrentAttackRange);
        foreach (var hitCollider in hitColliders){
            if(hitCollider == null) continue; // continue로 null 스킵
            if (hitCollider.TryGetComponent<IDamage>(out var target))
            {
                target.TakeDamage((int)_playerStatus.CurrentAttackDamage);
                // PlayAttackSound();
                Debug.Log($"공격 성공: {hitCollider.name}");
            }
        }
    }
    
    /// <summary>
    /// 앉기 처리
    /// </summary>
    private void OnSit()
    {
        if(_inputActions.isSit){
            
            if(!_isSitVisible){
                _isSitVisible = true;

                if (_hasFPSAnimator)
                {
                    _FPSAnimator.SetBool(_animIDSit, true);
                }

                if(_has3stAnimator){
                    _3stAnimator.SetBool(_animIDSit, true);
                }
            }
        }else{
            _isSitVisible = false;

            if (_hasFPSAnimator)
            {
                _FPSAnimator.SetBool(_animIDSit, false);
            }

            if(_has3stAnimator){
                _3stAnimator.SetBool(_animIDSit, false);
            }
        }
    }

    /// <summary>
    /// 일정 시간이 지난 후 공격 애니메이션을 종료하고 원래 상태로 복귀
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator ResetAttackAnimation(float delay)
    {
        // 공격 애니메이션이 끝날 때까지 기다림
        yield return new WaitForSeconds(delay);

        // 애니메이터가 존재하면 공격 애니메이션을 종료
        if (_hasFPSAnimator)
        {
            _FPSAnimator.SetBool(_animIDAttack, false);
        }

        if(_has3stAnimator){
            _3stAnimator.SetBool(_animIDAttack, false);
        }
    }
}


