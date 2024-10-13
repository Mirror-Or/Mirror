using System.Linq;
using UnityEngine;

public class CylinderSet : RaycastCheck, IInteractionable
{
    [Tooltip("실린더 순서대로 넣기")]
    [SerializeField] private SpinCylinder[] spinCylinder;     // 회전할 객체를 넣을 리스트
    
    [Tooltip("최대 회전 횟수")]
    public int[] cylinderSpinSet;           // 각 spinCylinder마다 회전할 수 있는 최대 회전 수
    
    [Tooltip("각 실린더 별 답")]
    [SerializeField] private int[] puzzleAnswer;              // 각 실린더 별로 맞춰져야하는 답
    
    [Tooltip("각 실린더 별 현재 답")]
    public int[] puzzleNowAnswer;           // 각 실린더의 현재 답

    [SerializeField] private bool test;     // 상호작용 테스트 용 변수   *임시*
    
    public float speed;   // 실린더들의 회전 속도
    [SerializeField] private Camera myCam;  // Raycast에 사용되는 카메라

    private bool interaction;
    
    private bool isOpen;                      // 답이 맞을 경우 true가 
    
    void Awake()
    {
        // puzzleNowAnswer의 리스트 수를 초기화
        puzzleNowAnswer = new int[puzzleAnswer.Length];
        
        // 각 실린더별 초기 세팅을 위한 반복문
        for (int i = 0; i < spinCylinder.Length; i++)
        {
            // 컴포넌트 받아오기
            spinCylinder[i] = spinCylinder[i].GetComponent<SpinCylinder>();
            // SpinCylinder의 순서를 i로 지정
            spinCylinder[i].thisCylinderNum = i;
            // puzzleNowAnswer의 현재 답을 0으로 세팅
            puzzleNowAnswer[i] = 0;
        }
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
            interaction = false;
        }
        
        if (!interaction) return;   // 상호 작용 중일때만 사용할 수 있도록 함
        
        // puzzleNowAnswer와 puzzleAnswer의 리스트 값이 같을 경우
        if (puzzleAnswer.SequenceEqual(puzzleNowAnswer))
        {
            // 클리어 판정
            isOpen = true;
            Debug.Log("Clear!");
        }
        // 좌클릭을 했을 때
        if (Input.GetMouseButtonDown(0))
        {
            CylinderClick();
        }
    }

    private void CylinderClick()
    {
        // 클릭한 객체가 SpinCylinder일 때 PuzzleClick을 실행함
        RayHitCheck(Input.mousePosition, myCam).transform.GetComponent<SpinCylinder>()?.PuzzleClick();
    }
    // 퍼즐의 현재 답을 바꿈
    public void ChangeNowAnswer(int cylinderNum)
    {
        puzzleNowAnswer[cylinderNum]++;
        // 만약 기본 값이 현재값보다 같거나 작을 시 0으로 바꿈
        if (cylinderSpinSet[cylinderNum] <= puzzleNowAnswer[cylinderNum])
        {
            puzzleNowAnswer[cylinderNum] = 0;
        }
    }
    // 플레이어가 상호작용을 진행했을 경우
    public void Interaction()
    {
        // 카메라를 켠다
        myCam.gameObject.SetActive(true);
        // 퍼즐을 풀 수 있도록 한다
        interaction = true;
    }
}
