using System.Collections;
using UnityEngine;

public class MoveMoniter : MonoBehaviour, IInteractionable
{
    [SerializeField] private float movePos = 1f;      // 한번에 움직이는 양

    private int _nowAngle;            // (-1 : 왼쪽, 0 : 중간, 1 : 오른쪽) 과 같은 방식으로 현재 위치 및 방향을 확인하는 용도
    private bool _move;              // 움직이는 중인지 확인
    private bool _leftMove;          // true일 때 왼쪽으로 이동, false일 때 오른쪽으로 이동

    private Vector3 _leftPos;        // 왼쪽으로 이동할 위치
    private Vector3 _rightPos;       // 오른쪽으로 이동할 위치
    private Vector3 _startPos;       // 시작 위치
    
    public enum MoveState           // 오브젝트를 회전 or 이동 용도로 사용할 것인지 선택
    {                               
        Move,
        Spin
    }
    public MoveState MoveStates;     // 오브젝트를 회전 or 이동 용도로 사용할 것인지 선택
    
    
    private void Start()
    {
        if(MoveStates == MoveState.Spin)
        {
            // 각도 변수들 초기화
            _startPos = transform.rotation.eulerAngles;
            _leftPos = new Vector3(0, _startPos.y - movePos, 0);
            _rightPos = new Vector3(0, _startPos.y + movePos, 0);
        }
        else
        {
            // 위치 변수들 초기화
            _startPos = transform.localPosition;
            _leftPos = new Vector3(transform.localPosition.x + movePos, 0, 0);
            _rightPos = new Vector3(transform.localPosition.x - movePos, 0, 0);
        }
    }

    void Update()
    {
        // move가 true일 때만 진행
        if (!_move) return;
        Move();
    }
    // Update에서 실행
    private void Move()
    {
        switch (MoveStates)
        {
            case MoveState.Move:
                if (_nowAngle == 0) // nowAngle이 0일 때 
                {
                    // 현재 각도를 startPos로 변경
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, _startPos, 0.25f);
                }
                else // nowAngle이 0이 아닐 때 
                {
                    // leftMove가 true일 땐 localPosition을 leftDir로 변경
                    // leftMove가 false일 땐 localPosition을 rightDir로 변경
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, _leftMove ? _leftPos : _rightPos, 0.25f);
                }
                break;
            case MoveState.Spin:
                if (_nowAngle == 0) // nowAngle이 0일 때 
                {
                    // 현재 각도를 startRot로 변경
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(_startPos), 1);
                }
                else // nowAngle이 0이 아닐 때 
                {
                    // leftMove가 true일 땐 각도를 leftRot로 변경
                    // leftMove가 false일 땐 각도를 rightRot로 변경
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(_leftMove ? _leftPos : _rightPos), 1);
                }
                break;    
        }
    }

    // 플레이어와의 상호작용으로 실행
    public void Interaction()
    {
        if (_move) return;   // move가 false일 때 진행
        if (_leftMove)   // leftMove가 true일 때
        {
            // nowAngle을 왼쪽으로 1칸 이동
            _nowAngle -= 1;
        }
        else    // leftMove가 false일 때
        {
            // nowAngle을 오른쪽으로 1칸 이동
            _nowAngle += 1;
        }
        _move = true;    // 이동 중으로 변경
        StartCoroutine(WaitTime()); //WaitTime 코루틴 실행
    }

    // PuzzleClick이 실행
    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(0.5f);  //실행 후 0.5초 대기
        _move = false;   // 움직임 중지
        if (_nowAngle != 0)  // nowAngle이 0이 아닐 시
        {
            // 왼쪽 <-> 오른쪽으로 교체
            _leftMove = !_leftMove;
        }
        WaitTimeMove();
    }
    // WaitTime에서 실행
    private void WaitTimeMove()
    {
        if(MoveStates == MoveState.Move)
        {
            // nowAngle에 따라 어긋난 위치 재조정
            transform.localPosition = _nowAngle switch
            {
                -1 => //  leftPos로 재조정
                    _leftPos,
                
                0 => // startPos로 재조정
                    _startPos,
                
                1 => // rightPos로 재조정
                    _rightPos,
                
                _ => transform.localPosition
            };
        }
        else
        {
            // nowAngle에 따라 어긋난 각도 재조정
            transform.rotation = _nowAngle switch
            {
                -1 => // leftPos로 재조정
                    Quaternion.Euler(_leftPos),
                
                0 => // startPos로 재조정
                    Quaternion.Euler(_startPos),
                
                1 => // RightPos로 재조정
                    Quaternion.Euler(_rightPos),
                
                _ => transform.rotation
            };
        }
    }
}
