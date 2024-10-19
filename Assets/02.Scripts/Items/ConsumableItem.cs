using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 사용할 수 있는 소모성 아이템관련 클래스
/// </summary>
public class ConsumableItem : ItemBase
{
    /// <summary>
    /// 아이템 사용 시 호출되는 메서드
    /// </summary>
    public override void Use()
    {
        if (Quantity > 0)
        {
            Quantity--;
            Debug.Log($"{ItemName}를 사용했습니다. 남은 수량: {Quantity}");

            if (Quantity <= 0)
            {
                DestroyItem();
            }
        }
        else
        {
            Debug.Log($"{ItemName}이 더 이상 남아있지 않습니다.");
        }
    }

    /// <summary>
    /// 아이템 개수가 0이 되었을 때 호출되는 메서드
    /// </summary>
    protected void DestroyItem()
    {
        Debug.Log($"{ItemName}이 모두 소모되었습니다.");
        // 아이템 소멸 처리
    }

    public override void Drop()
    {
        Debug.Log($"{ItemName}를 드롭하였습니다.");
        // 드롭 관련 로직 추가
    }

    public override void Pickup()
    {
        Debug.Log($"{ItemName}를 주웠습니다.");
        // 줍기 관련 로직 추가
    }
}