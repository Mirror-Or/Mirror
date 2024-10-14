using System.Collections;
using UnityEngine;

public class SpinCylinder : MonoBehaviour
{
    private bool _isSpinAngle;                // Y축 회전을 사용할 것인가
    private float _spinRotate;   // 한 번에 회전하는 각도
    private bool _isSpin;          // 현재 회전 중인지 확인하는 용도
    private float _speed;        // CylinderSet이 지정한 회전 속도를 받아오는 용도
    private float _waitTime;     // 회전을 할 때 기다리는 시간
    
    private int _cylinderNum;
    private int _nowAnswer;
    private float _cylinderSpinSet;

    public void Init(float spinSpeed, float spinSet, bool spinAngle)
    {
        // 초기 값 세팅
        _speed = spinSpeed;
        _cylinderSpinSet = spinSet;
        _isSpinAngle = spinAngle;
    }

    private void Start()
    {
        _waitTime = 1f / _speed;
        // CylinderSet이 설정한 면 수에 맞춰서 회전 각도를 지정함
        _spinRotate = 360f / _cylinderSpinSet;
    }

    // CylinderSet에서 실행시킴
    public int PuzzleClick()
    {
        // 회전 중이 아닐 때 (연속 회전 방지)
        if (_isSpin) return _nowAnswer;
        
        _isSpin = true;
        // 회전 코루틴 실행
        StartCoroutine(Spin());
            
        // CylinderSet에서 가지고 있는 이 객체의 현재 회전 값을 올림
        _nowAnswer++;
        // 만약 기본 값이 현재값보다 같거나 작을 시 0으로 바꿈
        if (_cylinderSpinSet <= _nowAnswer)
        {
            _nowAnswer = 0;
        }
        return _nowAnswer;
    }

    private void Update()
    {
        if(!_isSpin) return;
        
        if (!_isSpinAngle) // y축 회전이 아닐 때
        {
            // 정해진 각도만큼 회전
            transform.Rotate(_spinRotate * Time.deltaTime * _speed, 0, 0);
        }
        else // y축 회전일 때
        {
            // 정해진 각도만큼 회전
            transform.Rotate(0, _spinRotate * Time.deltaTime * _speed, 0);
        }
    }

    private IEnumerator Spin()
    {
        // waitTime 이상의 시간이 지났을 경우 다음 코드를 진행
        yield return new WaitForSeconds(_waitTime);

        if (!_isSpinAngle)     // y축 회전이 아닐 때
        {
            // 어긋난 각도 재조정
            transform.localRotation = Quaternion.Euler(_spinRotate * _nowAnswer, 0, 0);
        }
        else        // y축 회전일 때
        {
            // 어긋난 각도 재조정
            transform.localRotation = Quaternion.Euler(0, _spinRotate * _nowAnswer, 0);
        }
        
        // spin 초기화
        _isSpin = false;
    }
}
