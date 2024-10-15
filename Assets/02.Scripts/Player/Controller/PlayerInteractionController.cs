using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 상호작용을 담당하는 클래스
/// </summary>
public class PlayerInteractionController : MonoBehaviour
{   
    // 필요 컴포넌트 및 클래스
    private PlayerObjectDetectedController _objectDetectedController; // 감지한 오브젝트를 처리하는 컨트롤러

    // 감지된 오브젝트
    private GameObject detectedObject;
    private IInventoryItem _inventoryItem;

    public PlayerInteractionController(PlayerObjectDetectedController detectedController){
        _objectDetectedController = detectedController;
    }

    public void HandleInteraction(){
        detectedObject = _objectDetectedController.GetDetectedObject;

        if(detectedObject == null) return;

        // @todo: 추후 상호작용 가능한 오브젝트를 구분하는 로직 추가
        // ex) Item 습득, 문 열기, 벨브 열기 등
        _inventoryItem = detectedObject.GetComponent<IInventoryItem>();
        if(!_inventoryItem.IsPickable) {
            Debug.Log("획득할 수 없는 아이템입니다.");
            return;
        }

        // 아이템 획득
        detectedObject.SetActive(false);
        _inventoryItem.IsActive = false; 

        // 추후 인벤토리에 추가 로직 구현 필요

        // 추후 아이템 획득 사운드 재생

        Debug.Log("아이템 획득 완료");

    }

}
