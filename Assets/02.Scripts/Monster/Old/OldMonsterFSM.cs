using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class OldMonsterFSM : OldMonster,IDamage
{
    public GameObject MovePositionGroup;              // 몬스터의 탐색경로 그룹
    public bool IsMovingMonster = true;               // 학생(몬스터)가 움직일지 여부
    public List<Vector3> MoveDirectionList = new List<Vector3>();           // 학생(몬스터)의 탐색 경로 지정리스트
    public List<float> MoveDirectionDelayList = new List<float>();        // 학생(몬스터)각 경로에서 몇초 동안 멈출지 지정리스트
    public enum MonsterState       // 몬스터의 FSM
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }
    
    //---------------------------------------------------------------------
    protected float currentTime = 0f;    // 학생(몬스터)의 현재 공격시간 currentTime이 Delay보다 커진다면 공격 진행
    private bool _isDamaged = false;        // 학생(몬스터)의 피격확인용 변수
    //---------------------------------------------------------------------
        
    protected Transform player;          // 플레이어의 위치 값 받아오는 용도
    private Animator _animator;         // 학생(몬스터)의 애니메이터
    private bool _hasAnimator = false;  // 애니메이터가 있는지 확인용 변수
    private NavMeshAgent _navMeshA;     // 학생(몬스터)의 네비매쉬매니저

    protected MonsterState mState;               // 학생(몬스터)의 현재 상태

    
    private bool _isWait = false;                                       // 학생(몬스터) 지금 대기중인지 확인용 변수
    private int _moveDirectionIndex = 0;                                // 학생(몬스터)현재 경로 인덱스
    [SerializeField] private bool debugMode = false;                    // 학생(몬스터)의 탐색범위 가시화 할지 여부
    [Range(0f, 360f)] [SerializeField] private float viewAngle = 0f;    // 학생(몬스터)의 탐색범위(시야각)
    [SerializeField] private float viewRadius = 2f;                     // 학생(몬스터)의 탐색범위(반지름)
    [SerializeField] private LayerMask targetMask;                      // 학생(몬스터)의 탐색대상 레이어
    private LayerMask _obstacleMask;                            // 학생(몬스터)의 탐색대상 레이어2
    private Collider[] _targets = new Collider[10];

    private readonly Vector3[] _directionCache = new Vector3[3];        // 기즈모 그릴 방향

    /// <summary>
    /// 몬스터의 탐색경로 그룹 Test
    /// </summary>
    
    // Start is called before the first frame update
    private void Awake()
    {
        
        // 기즈모 방향 구하기
        _directionCache[0] = AngleToDir(transform.eulerAngles.y + viewAngle * 0.5f);
        _directionCache[1] = AngleToDir(transform.eulerAngles.y - viewAngle * 0.5f);
        _directionCache[2] = AngleToDir(transform.eulerAngles.y);
        //---------------------------------------------------------------------
        // 초기화들
        _navMeshA = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        // _player = GameObject.FindWithTag("Player").transform; 
        _hasAnimator = _animator != null;
        _navMeshA.speed = moveSpeed;
        StartPosition = transform.position;
        //--------------------------------------------------------------------- 
        // 움직일 위치 세팅
        if(MovePositionGroup != null) 
        {
            MoveDirectionList.Clear();
            for (int i = 0; i < MovePositionGroup.transform.childCount; i++)
            {
                MoveDirectionList.Add(MovePositionGroup.transform.GetChild(i).position);
                
            }
        }
        if (MoveDirectionDelayList.Count == 0)
        {
            for (int i = 0; i < MovePositionGroup.transform.childCount; i++)
            {
                MoveDirectionDelayList.Add(1.0f);
            }
        }
        //--------------------------------------------------------------------- Test
        if(_hasAnimator) StartCoroutine(WaitIdle(_moveDirectionIndex));
    }
 

    // 탐색 범위 가시화
    private void OnDrawGizmos() 
    {
        if (!debugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 0.5f - Vector3.back * 0.25f;
        Gizmos.DrawWireSphere(myPos, viewRadius);
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + viewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - viewAngle * 0.5f);
        
        Debug.DrawRay(myPos, rightDir * viewRadius, Color.blue,0);
        Debug.DrawRay(myPos, leftDir * viewRadius, Color.blue,0);
        Debug.DrawRay(myPos, transform.forward * viewRadius, Color.cyan,0);
    }
    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

    // Update is called once per frame
    private void Update()
    {
        if (player == null)
        {//아직 몬스터 매니저에서 소환하지 않아 플레이어가 생성되지 않은 상태여서 임시 처리
            player = GameObject.FindWithTag("Player").transform; 
        }
        switch (mState)// 스태이트머신
        {
            case MonsterState.Damaged:
                return;
            case MonsterState.Idle:
                Idle();
                break;
            case MonsterState.Move:
                Move();
                break;
            case MonsterState.Attack:
                Attack();
                break;
            case MonsterState.Die:
                Die();
                break;
        }
    }

   
    private IEnumerator WaitIdle(int index) //      학생(몬스터)가 각 루트끝에 몇 초 동안 대기하고 다음 루트를 지정하는 함수
    {
        _isWait = true; //  기다림 시작
        _animator.SetTrigger("toIdle"); //                          애니메이션 변경
        yield return new WaitForSeconds(MoveDirectionDelayList[index]); //   기다림
        _animator.SetTrigger("toWork"); //                          애니메이션 변경
        _navMeshA.SetDestination(MoveDirectionList[_moveDirectionIndex]);   //  다음 루트 지정
        _moveDirectionIndex = (_moveDirectionIndex + 1) % MoveDirectionList.Count;  
        _isWait = false; // 기다림 끝
    }
    
    protected virtual void  Idle()  //  학생(몬스터) 탐색
    {
        if (IsMovingMonster && _navMeshA.remainingDistance <= _navMeshA.stoppingDistance) // 몬스터의 거리가 목적지와 가깝다면
        {
            if (!_isWait) //    대기 상태가 아니라면
            {
                StartCoroutine(WaitIdle(_moveDirectionIndex)); //   WaitIdle 함수 시작
            }
            // Debug.Log(_moveDirectionIndex);
        }
        //  시야 적용 방식
        Surveilance();
    }

    private void Surveilance()
    {
        Vector3 myPos = transform.position + Vector3.up * 0.5f;     //      레이 시작 위치 
        // Collider[] Targets = Physics.OverlapSphere(myPos, viewRadius, TargetMask);  //  원형 범위 내 레이어 검출
        int numColliders = Physics.OverlapSphereNonAlloc(myPos, viewRadius, _targets, targetMask);
        if (_targets.Length == 0)    //                  아무것도 안 검출되면 리턴
            return;
        // Debug.Log(Targets[0].name);
        for (int i = 0; i < numColliders; i++)
        {
            Collider enemyCollider = _targets[i];
            Vector3 targetPos = enemyCollider.transform.position + new Vector3(0, 2, 0);
            Vector3 targetDir = (targetPos - myPos).normalized;
            float targetAngle = Mathf.Acos(Vector3.Dot(transform.forward, targetDir)) * Mathf.Rad2Deg;
            if (targetAngle <= viewAngle * 0.5 && !Physics.Raycast(myPos,targetDir,viewRadius,_obstacleMask))
            {
                Debug.Log("플레이어 감지");
                if (enemyCollider.gameObject.CompareTag("Player"))//   만약 레이를 맞은 오브젝트가 플레이어라면
                {
                    _animator.SetTrigger("toWork");     //      애니메이션 변경
                    mState = MonsterState.Move;    //              학생(몬스터)의 상태를 Move로 변경
                }
                if (debugMode) Debug.DrawLine(myPos, targetPos, Color.red); //  레이를 시각화 함
            }
        }
    }

    protected virtual void Move()  //  학생(몬스터)추격
    {
        float searchDistance = _isDamaged ? viewRadius * 3 : viewRadius * 2; 
        // 학생(몬스터)가 Damage로 Move가 됬는지 Idle에서 Move로 됬는지 만약 Damage라면 추격 범위가 탐색범위 * 3 아니면 탐색범위 * 2
        if (Vector3.Distance(player.position, transform.position) > searchDistance) // 플레이어와 학생(몬스터)의 거리가 탐색범위 보다 길다면
        {
            Debug.Log("d?" + searchDistance);
            _navMeshA.SetDestination(StartPosition); // 시작 위치로 돌아감
            mState = MonsterState.Idle;              // 애니메이션 변경 
            /*a_nim.SetTrigger("toWork");*/
            _isDamaged = false;
        }
        else if (Vector3.Distance(player.position, transform.position) > attackDistance) 
            // 만약 플레이어의와의 거리가 공격범위보다 멀지만 탐색 벗어나는 범위보다 가깝다면
        {
            _navMeshA.destination = player.position;   // 추격함 
        }
        else
        {   // 만약 플레이어와 학생(몬스터)의 거리가 공격 범위보다 가깝다면 
            mState = MonsterState.Attack;// 학생(몬스터)의 상태를 Attak으로 변경
            currentTime = attackDelay; // 처음 1회 공격 바로 시전
        }
    }

    public virtual void Attack()        //  학생(몬스터)의 플레이어 공격
    {                           //  만약 플레이어와 학생(몬스터)의 거리가 공격 범위 내라면
        if (Vector3.Distance(player.position, transform.position) < attackDistance)
        {                       //  currentTime 카운트 시작
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)// currentTime이 attackDelay만큼 카운트 했다면 공격 진행
            {
                Debug.Log("공격");
                currentTime = 0;       // currentTime 초기화
            }
        }
        else
        {               //  플레이어와 학생(몬스터)의 거리가 공격 범위 보다 멀다면
            mState = MonsterState.Move;    // 학생(몬스터)의 상태를 Move로 변경
            currentTime = 0;   // currentTime 초기화
        }
    }

    public void TakeDamage(int hitPower)    //  학생(몬스터)의 피격
    {
        if (mState == MonsterState.Damaged || mState == MonsterState.Die) return; 
        if (monsterHp > 0)  
            //                                  체력이 0 or 죽은 상태가 아니라면
        {
            monsterHp -= hitPower;  // 학생(몬스터)의 체력을 뺌
            mState = MonsterState.Damaged;// 학생(몬스터)의 상태를 Damaged 변경
            _isDamaged = true;             //피격 상태 
            StartCoroutine(WaitDamage());// WaitDamage 함수 호출


            Debug.Log($"현재 Monster 체력 : {monsterHp}");
        }
        else
        {
            mState = MonsterState.Die;
        }
    }

    private IEnumerator WaitDamage()        // 피격 되는 시간동안 움직이지 못하게 하는 함수
    {
        
        yield return new WaitForSeconds(1f);//  1초동안 대기함
        mState = MonsterState.Move;        //  학생(몬스터)의 상태를 Move 변경
    }

    protected virtual void Die()   // 학생(몬스터)를 죽음처리(없앰)함
    {
        gameObject.SetActive(false);
    }
}
