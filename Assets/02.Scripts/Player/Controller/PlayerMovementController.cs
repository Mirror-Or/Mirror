using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 이동을 담당하는 클래스
/// </summary>
public class PlayerMovementController
{
    // 필요 컴포넌트 및 클래스
    private CharacterController _characterController;
    private PlayerAnimationController _animationController;
    
    // 이동 관련 변수
    private float _currentSpeed;             // 현재 플레이어의 이동 속도
    private float _horizontalSpeed;          // 캐릭터의 수평 방향 이동 속도 (X, Z 방향 속도)
    private Vector3 _moveDirection;          // 현재 이동 방향 (X, Z 방향)
    private float _targetSpeed;              // 목표 이동 속도 (걷기 또는 달리기 속도)
    private float _speedOffset;              // 이동 속도 변경을 위한 오프셋 값 (속도 변화 허용 범위)
    private float _walkSpeed;                // 걷기 속도
    private float _runSpeed;                 // 달리기 속도
    private float _speedChangeRate;          // 이동 속도 변경 비율 (속도 전환 속도)

    // 점프 관련 변수
    private bool _isGrounded;                // 캐릭터가 땅에 붙어 있는지 여부
    private float _verticalVelocity;         // 수직 속도 (점프 및 낙하 시 속도)
    private Vector3 _verticalMovement;       // 수직 이동 방향
    private float _gravity;                  // 중력 값
    private float _jumpTimeout;              // 점프 후 재점프 가능 시간 (딜레이)
    private float _fallTimeout;              // 낙하 시 발생하는 딜레이 시간
    private float _jumpHeight;               // 점프 높이
    private float _terminalVelocity;         // 캐릭터가 떨어질 때 최대 속도 (종단 속도)    

    // 땅 체크 관련 변수
    private float _groundedRadius;           // 땅 체크 반지름

    // 애니메이션 관련 변수
    private float _animationBlend;           // 애니메이션 블렌드 값 (이동 속도에 따른 애니메이션 전환 비율)

    public bool IsGrounded => _isGrounded;
    public Vector3 CurretPosition => _characterController.transform.position;

    public PlayerMovementController(CharacterController characterController, PlayerAnimationController animationController)
    {
        _characterController = characterController;
        _animationController = animationController;

        InitializeMovement();
    }

    private void InitializeMovement(){
        // 이동 관련 변수 초기화
        _currentSpeed = 0.0f;
        _horizontalSpeed = 0.0f;
        _moveDirection = Vector3.zero;
        _targetSpeed = 0.0f;
        _speedOffset = 0.1f;
        _speedChangeRate = PlayerBasicSettings.speedChangeRate;
        _walkSpeed = PlayerBasicSettings.walkSpeed;
        _runSpeed = PlayerBasicSettings.runSpeed;
        _animationBlend = 0.0f;

        // 점프 관련 변수 초기화
        _isGrounded = false;
        _verticalVelocity = 0.0f;
        _verticalMovement = Vector3.zero;
        _gravity = -9.81f;
        _jumpTimeout = 0.0f;
        _fallTimeout = 0.0f;
        _jumpHeight = PlayerBasicSettings.jumpHeight;
        _terminalVelocity = 53.0f;

        _groundedRadius = 0.28f;
    }

    // 플레이어의 이동을 처리하는 함수
    public void HandleMovement(Vector2 inputDirection, bool isSprinting )
    {
        // 이동 속도 결정
        _targetSpeed = isSprinting ? _runSpeed : _walkSpeed;
        if(inputDirection == Vector2.zero) _targetSpeed = 0.0f;

        // 이동 방향 계산
        _moveDirection.x = inputDirection.x;
        _moveDirection.z = inputDirection.y;
        _moveDirection = _characterController.transform.TransformDirection(_moveDirection).normalized;

        // 현재 수평 속도 계산 (velocity 대신 이동 방향과 속도 사용)
        _horizontalSpeed = new Vector3(_moveDirection.x, 0.0f, _moveDirection.z).magnitude * _currentSpeed;

        // Debug.Log($"currentHorizontalSpeed : {_horizontalSpeed}");

        // 목표 속도와 현재 속도의 차이를 확인하여 가속 또는 감속을 처리
        if(Mathf.Abs(_horizontalSpeed - _targetSpeed) > _speedOffset)
        {
            _currentSpeed = Mathf.Lerp(_horizontalSpeed, _targetSpeed, Time.deltaTime * _speedChangeRate);
        }
        else
        {
            _currentSpeed = _targetSpeed;
        }

        // 캐릭터 이동 처리
        if(_currentSpeed > 0 || Mathf.Abs(_verticalVelocity) > 0)
        {
            // verticalMovement 재사용
            _verticalMovement.Set(0, _verticalVelocity, 0);
            _characterController.Move(_moveDirection * (_currentSpeed * Time.deltaTime) + _verticalMovement * Time.deltaTime);
        }

        // 애니메이션 처리
        HandleMovementAnimation(_currentSpeed, _speedChangeRate);
    }

    private void HandleMovementAnimation(float targetSpeed, float speedChangeRate)
    {
        // 애니메이션 블렌드를 계산 (이동 속도에 따른 애니메이션 변경)
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);

        // AnimationController에 애니메이션 정보를 전달
        _animationController.SetAnimationFloat(AnimatorParameters.Speed, _animationBlend);
    }

    #region 점프 및 낙하 처리
    public void HandleJump(bool isJump)
    {
        Debug.Log($"HandleJump : {_isGrounded}");
        // 플레이어가 땅에 붙어있는 경우와 공중에 있는 경우를 나눠 처리
        if (_isGrounded)
        {
            HandleGroundedJump(isJump);  // 땅에 붙어 있을 때의 점프 로직 처리
        }
        else
        {
            HandleAirborne();  // 공중에 있을 때의 낙하 로직 처리
        }

        MoveCharacter(); // 최종적으로 플레이어 이동 처리
    }

    // 땅에 붙어 있을 때의 점프 처리 함수
    private void HandleGroundedJump(bool isJump)
    {
        // 낙하 타임아웃 초기화 (플레이어가 다시 땅에 닿을 때까지의 시간)
        _fallTimeout = 0.15f;
        
        // 점프 애니메이션 종료
        _animationController.SetAnimationBool(AnimatorParameters.IsJumping, false);

        // 플레이어가 점프 버튼을 눌렀고, 점프 타임아웃이 끝난 상태라면 점프 실행
        if (isJump && _jumpTimeout <= 0.0f)
        {
            // 점프 높이, 중력 및 보정 값을 이용한 수직 속도 계산 (점프 동작)
            _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);

            // 점프 애니메이션 시작
            _animationController.SetAnimationBool(AnimatorParameters.IsJumping, true);
        }

        // 점프 타임아웃 처리
        if (_jumpTimeout >= 0.0f)
        {
            _jumpTimeout -= Time.deltaTime;
        }
    }

    // 공중에 있을 때 낙하 처리 함수
    private void HandleAirborne()
    {
        // 점프 타임아웃 초기화 (공중에 있는 동안 다시 점프하지 않도록 제한)
        _jumpTimeout = 0.5f;

        // 낙하 타임아웃 감소 (플레이어가 떨어지는 중)
        if (_fallTimeout >= 0.0f)
        {
            _fallTimeout -= Time.deltaTime;
        }

        // 공중에서의 애니메이션 처리
        _animationController.SetAnimationBool(AnimatorParameters.IsJumping, false);
    }

    // 중력 적용 및 수직 속도 계산 함수
    private void ApplyGravity()
    {
        // 현재 수직 속도가 터미널 속도에 도달하지 않았다면 중력 적용
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }

    // 캐릭터 이동 처리 함수
    private void MoveCharacter()
    {
        // 수직 이동 벡터 설정 (0, 수직 속도, 0)
        _verticalMovement.Set(0, _verticalVelocity, 0);

        // 캐릭터 이동 처리 (프레임당 이동 거리 계산)
        _characterController.Move(_verticalMovement * Time.deltaTime);
    }

    /// <summary>
    /// 캐릭터가 땅에 붙어 있는지 여부를 체크하는 함수
    /// </summary>
    /// <param name="groundCheckPosition">캐릭터의 땅 체크 위치</param>
    /// <param name="groundCheckRadius">땅 체크 반지름</param>
    /// <param name="groundLayers">땅여부 확인 Layer</param>
    public void UpdateGroundedStatus(Vector3 groundCheckPosition, LayerMask groundLayers)
    {
        // 땅에 붙어 있는지 여부를 체크
        _isGrounded = Physics.CheckSphere(groundCheckPosition, _groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        ApplyGravity(); // 중력 적용

        // 애니메이션에 grounded 상태 반영
        _animationController.SetAnimationBool(AnimatorParameters.IsGrounded, _isGrounded);
    }
    #endregion
    
    // 앉기 처리    
    public void HandleSit(bool isSit){
        // 추후 이동속도 감소 등 추가 가능

        _animationController.SetAnimationBool(AnimatorParameters.IsSitting, isSit);
    }

}   
