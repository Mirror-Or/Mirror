using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CylinderSet : RaycastCheck, IInteractionable
{
    [Tooltip("최대 회전 횟수")]
    [SerializeField] private bool isYSpin;                // Y축 회전을 사용할 것인가
    [Tooltip("실린더 순서대로 넣기")]
    [SerializeField] private List<SpinCylinder> spinCylinder;     // 회전할 객체를 넣을 리스트
    [Tooltip("각 실린더 별 답")]
    [SerializeField] private List<int> puzzleAnswer;              // 각 실린더 별로 맞춰져야하는 답
    
    [SerializeField] private float speed;                     // 실린더들의 회전 속도
    [SerializeField] private bool test;     // 상호작용 테스트 용 변수   *임시*
    [SerializeField] private int[] CylinderSpinSet;           // 각 spinCylinder마다 회전할 수 있는 최대 회전 수
    [SerializeField] private Camera myCam;  // Raycast에 사용되는 카메라

    
    private bool _interaction;
    private List<int> _puzzleNowAnswer;           // 각 실린더의 현재 답
    private bool _isOpen;                      // 답이 맞을 경우 true가
    
    void Awake()
    {
        for (int i = 0; i < spinCylinder.Count; i++)
        {
            spinCylinder[i].Init(speed, CylinderSpinSet[i], isYSpin);
        }
        
        // puzzleNowAnswer의 리스트 수를 초기화
        _puzzleNowAnswer = Enumerable.Repeat(0, spinCylinder.Count).ToList();
    }
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
        
        // puzzleNowAnswer와 puzzleAnswer의 리스트 값이 같을 경우
        if (puzzleAnswer.SequenceEqual(_puzzleNowAnswer))
        {
            // 클리어 판정
            _isOpen = true;
            Debug.Log("Clear!");
            // 카메라를 끈다
            myCam.gameObject.SetActive(false);
            _interaction = false;
        }
        // 좌클릭을 했을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 클릭한 객체가 SpinCylinder일 때 PuzzleClick을 실행함
            var cylinderObj = RayHitCheck(Input.mousePosition, myCam).transform;
            var findIndex = spinCylinder.FindIndex(n => n.transform == cylinderObj);
            if (findIndex != -1)
            {
               _puzzleNowAnswer[findIndex] = cylinderObj.GetComponent<SpinCylinder>().PuzzleClick();
            }
            
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
