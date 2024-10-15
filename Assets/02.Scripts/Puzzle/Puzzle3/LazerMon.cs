using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LazerMon : MonoBehaviour
{
    [SerializeField] private float defaultLength = 50;        // 레이저의 길이
    [SerializeField] private float reflectNum = 20;           // 반사 가능 횟수

    private LayerMask _clearLayer = LayerMask.NameToLayer("Clear");
    private LayerMask _reflectLayer = LayerMask.NameToLayer("Reflect");
    
    private LineRenderer _lineRenderer;      // 레이저 표시용 LineRenderer 변수
    private RaycastHit _hit;               // 오브젝트 충돌 체크용 Raycast 변수
    
    
    private void Start()
    {
        // LineRenderer 컴포넌트 받아옴
        _lineRenderer = GetComponent<LineRenderer>();   
    }

    private void Update()
    {
        // 레이저 반사 함수 실행
        ReflectLazer(); 
    }
    
    // 레이저 반사 함수
    private void ReflectLazer()
    {
        // Ray를 생성
        var ray = new Ray(transform.position, transform.forward);

        // LineRenderer의 다음 도착 지점을 1로 설정
        _lineRenderer.positionCount = 1;
        // LineRenderer 시작점 지정
        _lineRenderer.SetPosition(0, transform.position);

        // Raycast의 길이 변수를 defaultLength로 지정
        var resetLen = defaultLength;

        // 반사되는 횟수만큼 반복
        for (int i = 0; i < reflectNum; i++)
        {
            // LineRenderer의 다음 지점을 추가
            _lineRenderer.positionCount += 1;
            if (Physics.Raycast(ray.origin, ray.direction, out _hit, resetLen))
            {
                LayerMask layer = _hit.transform.gameObject.layer;
                
                if (layer == _reflectLayer)
                {
                    // ray를 충돌 지점에서 반사각만큼 회전한 방향으로 재생성
                    ray = new Ray(_hit.point, Vector3.Reflect(ray.direction, _hit.normal));
                }
                else if (layer== _clearLayer)
                {
                    ClearEvent();
                }
                // LineRenderer를 이전 위치에서 충돌 지점까지 그림
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _hit.point);
            }
            else
            {
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, ray.origin + (ray.direction * resetLen));
            }
        }
    }

    private void ClearEvent()
    {
        Debug.Log("Clear");
    }
}
