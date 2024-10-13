using UnityEngine;

public class RaycastCheck : MonoBehaviour
{
    /// <summary>
    /// Bool 값을 반환해줌
    /// </summary>
    protected bool RayHitCheck(Vector3 mousePosition, Camera myCam, Transform target)
    {
        // 카메라의 마우스 위치에서 Ray를 생성
        Ray myRay = myCam.ScreenPointToRay(mousePosition);
        
        // Ray가 물체와 충돌했을 시 true, 아니면 false
        bool isHit = Physics.Raycast(myRay, out var raycastHit);
        return isHit && raycastHit.transform == target;
    }
    /// <summary>
    /// Transform 값을 반환해줌
    /// </summary>
    protected Transform RayHitCheck(Vector3 mousePosition, Camera myCam)
    {
        // 카메라의 마우스 위치에서 Ray를 생성
        Ray myRay = myCam.ScreenPointToRay(mousePosition);
        
        // Ray가 물체와 충돌했을 시 true, 아니면 false
        Physics.Raycast(myRay, out var raycastHit);
        return raycastHit.transform;
    }
}
