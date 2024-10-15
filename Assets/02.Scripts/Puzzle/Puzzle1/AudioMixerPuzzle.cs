using System.Collections.Generic;
using UnityEngine;

public class AudioMixerPuzzle : RaycastCheck, IInteractionable
{
    [SerializeField] private float maxMove;               // 슬라이더가 상하로 이동할 수 있는 최대 거리
    [SerializeField] private float buttonMoveSpeed = 1;   // 버튼이 이동하는 속도
    [SerializeField] private List<GameObject> button;         // 버튼들을 담을 리스트
    [SerializeField] private Camera myCam;                // Raycast 및 화면 전환할 카메라
    [SerializeField] private bool isOpen;

    [SerializeField] private bool test;                    // 상호작용 테스트 용 변수   *임시*
    
    private bool _interaction;            // 상호 작용 확인
    private bool _isDrag;                   // 드래그 중인지 확인할 bool 값
    private int _nowDragButton;           // 현재 드래그 중인 버튼 확인 용도
    
    void Update()
    {
        if (test)       // 상호작용 테스트 용  *임시*
        {
            Interaction();
            test = false;
        }

        if (Input.GetKeyUp(KeyCode.Escape))      // 상호작용 테스트 용  *임시*
        {
            // 카메라를 끈다
            myCam.gameObject.SetActive(false);
            // 상호작용 종료
            _interaction = false;
        }
        
        if (!_interaction) return;   // 상호 작용 중일때만 사용할 수 있도록 함
        
        Drag(); // 드래그 감지
        ButtonMove(); // 버튼을 움직이는 용도
        // 클리어 확인 용도
    }

    private void Drag()
    {
        // 좌클릭을 눌렀을 때
        if(Input.GetMouseButtonDown(0)){
            _nowDragButton = button.FindIndex(n => n.transform == RayHitCheck(Input.mousePosition, myCam));
            if(_nowDragButton != -1)
            {
                _isDrag = true;
            }
        }
        
        // 좌클릭이 끝났을 때
        if(Input.GetMouseButtonUp(0)){
            // 드래그 중지
            _isDrag = false;
            CheckClear();
        }
    }

    private void ButtonMove()
    {
        // 현재 드래그 중인 버튼의 localPosition값을 받아옴
        var buttonPos = button[_nowDragButton].transform.localPosition;
        // 드래그 중일 때
        if(_isDrag)
        {
            // 마우스의 위치 값을 저장
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCam.WorldToScreenPoint(transform.position).z);
            
            // 카메라 기준으로 position 값을 저장
            Vector3 worldPosition = myCam.ScreenToWorldPoint(position);
            button[_nowDragButton].transform.localPosition = new Vector3(buttonPos.x, buttonPos.y, Mathf.Clamp(worldPosition.z * buttonMoveSpeed,-maxMove, maxMove));
        }
    }

    private void CheckClear()
    {
        var check = 0;          // 통과 조건을 만족시킨 버튼 개수 체크 용
        
        // 각 버튼마다 값을 확인
        foreach (var t in button)
        {
            // 현재 체크 중인 버튼의 위치가 최상단에 위치할 경우
            if (t.transform.localPosition.z >= maxMove)
            {
                // 통과한 버튼의 개수를 1개 추가
                check++;
            }
            else
            {
                // 아닐 경우에 반복문 종료
                break;
            }
        }

        // 모든 버튼이 통과 조건을 만족했을 시
        if (check == button.Count)
        {
            // 클리어로 변경함
            isOpen = true;
            Debug.Log("Clear");
            
            // 카메라를 끈다
            myCam.gameObject.SetActive(false);
            // 상호작용 종료
            _interaction = false;
        }
    }

    // 플레이어가 상호작용을 진행했을 경우
    public void Interaction()
    {
        // 카메라를 켠다
        myCam.gameObject.SetActive(true);
        // 퍼즐을 풀 수 있도록 한다
        _interaction = true;
    }
}
