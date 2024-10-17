using System.Collections.Generic;
using UnityEngine;

public class Scratch : RaycastCheck, IInteractionable
{
    [SerializeField] private List<GameObject> codeBlock;      // 코드 블록을 담을 리스트
    [SerializeField] private Camera myCam;                // Raycast 및 화면 전환할 카메라
    [SerializeField] private GameObject inputBlockPos;       // 코드 블록이 들어갈 위치
    [SerializeField] private GameObject playButton;       // 실행 버튼

    [SerializeField] private bool test;                    // 상호작용 테스트 용 변수   *임시*

    private GameObject _getCodeBlock;     // 실행할 코드 블럭
    private Vector3[] _codeBlockSetPos;   // 코드 블록들의 시작 위치
    private bool _interaction;            // 상호 작용 확인
    private bool _isDrag;                   // 드래그 중인지 확인할 bool 값
    private int _nowDragButton;           // 현재 드래그 중인 버튼 확인 용도

    private void Start()
    {
        // 코드 블록 기본 위치 세팅
        _codeBlockSetPos = new Vector3[codeBlock.Count];
        for(int i = 0; i < codeBlock.Count; i++)
        {
            _codeBlockSetPos[i] = codeBlock[i].transform.localPosition;
        }
    }

    private void Update()
    {
        if (test)       // 상호작용 테스트 용  *임시*
        {
            Interaction();
            test = false;
        }

        if (Input.GetKeyUp(KeyCode.Escape))      // 상호작용 테스트 용  *임시*
        {
            EndInteraction();
        }
        
        if (!_interaction) return;   // 상호 작용 중일때만 사용할 수 있도록 함
        
        Drag(); // 드래그 감지
        BlockMove(); // 버튼을 움직이는 용도
    }
    private void Drag()
    {
        // 좌클릭을 눌렀을 때
        if(Input.GetMouseButtonDown(0)){
            // 누른게 실행 버튼일 때 
            if (RayHitCheck(Input.mousePosition, myCam, playButton.transform))
            {
                // 만약 코드 블록이 들어가 있을 때
                if (_getCodeBlock != null)
                {
                    // 코드 블록에 따른 이벤트를 실행한다.
                    PlayCode(_getCodeBlock);
                }
                // 아니면
                else
                {
                    // 에러를 띄운다
                    Debug.Log("Error");
                }
            }

            _nowDragButton = codeBlock.FindIndex(n => n.transform == RayHitCheck(Input.mousePosition, myCam));
            if(_nowDragButton != -1)
            {
                if (_getCodeBlock == codeBlock[_nowDragButton]) _getCodeBlock = null;
                _isDrag = true;
            }
        }
        
        // 좌클릭이 끝났을 때
        if(Input.GetMouseButtonUp(0)){
            // 드래그 중지
            _isDrag = false;
            // 코드 블록과 Input Block의 거리를 잰다
            var distance = Vector3.Distance(codeBlock[_nowDragButton].transform.position, inputBlockPos.transform.position);

            // 일정 범위 내에 코드 블록이 떨어졌다면
            if (distance < 0.75f)
            {
                // 이미 Input Block안에 블록이 들어가 있을 경우
                if (_getCodeBlock != null)
                {
                    // Input Block 안의 코드 블록을 조금 이동시킨 뒤
                    _getCodeBlock.transform.localPosition += new Vector3(0.1f,0, 0.1f);
                    // 코드 블록의 부모를 바꿔준다
                    _getCodeBlock = null;
                }
                // 드랍한 코드 블록의 부모를 Input Block로 변경해준다.
                _getCodeBlock = codeBlock[_nowDragButton];
                // 코드 블록의 위치를 Input Block의 위치로 변경해준다.
                _getCodeBlock.transform.position = inputBlockPos.transform.position;
            }
            // 드래그 종료 시에 코드블록이 카메라를 벗어났을 경우
            if (!CheckInCam(codeBlock[_nowDragButton]))
            {
                // 코드 블록의 위치를 원래 위치로 되돌려준다.
                codeBlock[_nowDragButton].transform.localPosition = _codeBlockSetPos[_nowDragButton];
            }
        }
    }

    private void BlockMove()
    {
        // 현재 드래그 중인 블럭의 localPosition값을 받아옴
        var buttonPos = codeBlock[_nowDragButton].transform.localPosition;
        
        // 드래그 중일 때
        if (!_isDrag) return;
        // 마우스의 위치 값을 저장
        Vector3 position = new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, myCam.WorldToScreenPoint(transform.position).z);
            
        // 카메라 기준으로 position 값을 저장
        Vector3 worldPosition = myCam.ScreenToWorldPoint(position);

        // 블럭이 마우스를 따라가도록 함
        codeBlock[_nowDragButton].transform.localPosition = new Vector3(worldPosition.x, buttonPos.y, worldPosition.z);
    }

    private bool CheckInCam(GameObject dragBlock)
    {
        // dragBlock의 카메라 상 위치 값을 받아온다.
        Vector3 screenPoint = myCam.WorldToViewportPoint(dragBlock.transform.position);
        // dragBlock의 위치가 화면 안인지 체크해 bool 값으로 받아온다.
        bool inScreen = screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0; 
        // dragBlock이 화면 안에 존재하는지 아닌지 알려준다
        return inScreen;
    }
    
    // 상호작용
    public void Interaction()
    {
        // 상호작용 시작
        _interaction = true;
        // 카메라를 켠다
        myCam.gameObject.SetActive(true);
    }
    
    // 상호작용 종료
    private void EndInteraction()
    {
        // 카메라를 끈다
        myCam.gameObject.SetActive(false);
        // 상호작용 종료
        _interaction = false;
    }

    // 들어가 있는 코드 블록에 따라 이벤트를 실행한다.
    private void PlayCode(GameObject getCodeBlock)
    {
        // 나중에 이벤트 추가 시 주석처리 해제
        // getCodeBlock.GetComponent<ICodeBlockEvent>().PlayEvent();
        
        // 상호작용을 종료한다
        EndInteraction();
    }
}
