using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

/// <summary>
/// 카메라 컨트롤러 클래스
/// </summary>
public class PlayerCameraController
{
    private Transform _playerCamera;                // 플레이어 카메라
    private float _cinemachineTargetPitch = 0.0f;   // 현재 카메라 회전 각도
    private float _topClamp = 70.0f;                // 카메라 상단 회전 제한 각도
    private float _bottomClamp = -30.0f;            // 카메라 하단 회전 제한 각도
    private float _cameraAngleOverride = 0.0f;       // 카메라 각도 오버라이드

    // 마우스 관련 변수
    private Vector2 mousePosition;              // 마우스 위치

    public PlayerCameraController(Transform camera){
        _playerCamera = camera;

        mousePosition = new Vector2(Screen.width / 2f, Screen.height / 2f);

        MouseUtil.LockAndHideCursor(); // 마우스 커서 숨기기
    }

    /// <summary>
    /// 마우스 입력을 받아서 카메라 회전을 처리하는 함수
    /// </summary>
    /// <param name="rotation">입력받은 마우스 움직임 값</param>
    public void UpdateCameraRotation(float rotation){
        // 카메라를 마우스 입력에 따른 상하 회전을 적용
        _cinemachineTargetPitch -= rotation;

        // 카메라의 상하 회전 각도를 제한
        _cinemachineTargetPitch = MathUtil.ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
        Quaternion targetRotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride, 0.0f, 0.0f);
        
        _playerCamera.localRotation = targetRotation;
    }

    public void UpdateCameraPosition(Transform pointTR){
        _playerCamera.transform.position = pointTR.position  + pointTR.right * -0.2f;
    }

    /// <summary>
    /// 커서 상태를 설정하는 함수
    /// </summary>
    /// <param name="state"></param>
    public void SetCursorState(bool state)
    {
        if(state){
            MouseUtil.UnlockAndShowCursor();
        }else{
            MouseUtil.LockAndHideCursor();
        }
    }

    public Vector2 GetMousePosition => mousePosition;

}
