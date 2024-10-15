using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectDetectedController
{
    private Camera _playerCamera;           // 플레이어 카메라
    private RaycastHit _hit;                // 레이캐스트 히트 정보
    private float _maxDistance;             // 레이캐스트 최대 거리
    private GameObject _detectedObject;     // 감지된 오브젝트
    private Outline _targetOutline;         // 아웃라인 캐싱

    private Vector2 _screenCenter;           // 화면 중앙 위치

    public PlayerObjectDetectedController(Camera camera, float maxDistance){
        _playerCamera = camera;
        _maxDistance = maxDistance;
        _screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }
    
    /// <summary>
    /// 플레이어가 감지한 오브젝트를 처리하는 함수
    /// </summary>
    public void DetectObject(){
        // 현재는 마우스가 게임의 중앙에 위치하도록 설정되어 있기에 ray를 생성할 때 screenCenter를 사용
        // 만일 마우스 입력을 받는다면 마우스 위치를 받아서 처리하도록 수정 필요
        Ray ray = _playerCamera.ScreenPointToRay(_screenCenter);

        // 레이캐스트를 통해 오브젝트를 감지하고 처리
        // @Todo: 추후 오브젝트 타입에 따라 다른 처리를 할 수 있도록 수정 필요
        if (Physics.Raycast(ray, out _hit, _maxDistance))
        {
            HandleDetectedObject(_hit.collider.gameObject);
        }
        else
        {
            ResetDetectedItem();
        }
    }

    /// <summary>
    /// 감지된 오브젝트를 처리하는 메서드
    /// </summary>
    /// <param name="obj">감지된 오브젝트</param>
    private void HandleDetectedObject(GameObject obj)
    {
        if (_detectedObject != obj)
        {
            ResetDetectedItem();
            _detectedObject = obj;

            _targetOutline = _detectedObject.GetComponent<Outline>();
            _targetOutline?.SetOutline(true);
        }
    }

    /// <summary>
    /// 감지된 오브젝트를 초기화하는 메서드
    /// </summary>
    private void ResetDetectedItem()
    {
        if (_detectedObject != null)
        {
            _targetOutline?.SetOutline(false);
            _detectedObject = null;
        }
    }

    /// <summary>
    /// 감지된 오브젝트 반환
    /// </summary>
    public GameObject GetDetectedObject => _detectedObject;
}
