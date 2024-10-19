using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Medical,
    Quest,
    Functional,
    Weapon
}

public class ItemData
{
    public string itemID;
    public string itemName;
    public string itemDiscription;
    public ItemType itemType;
    public string itemIconPath;
    public int quantity;
}

/// <summary>
/// 아이템의 기본적인 속성과 동작을 정의하는 클래스
/// </summary>
public abstract class ItemBase : MonoBehaviour, IInteractable
{
    public string ItemID { get; protected set; }            // 아이템 ID
    public string ItemName { get; protected set; }          // 아이템 이름
    public string ItemDiscription { get; protected set; }   // 아이템 설명
    public ItemType ItemType { get; protected set; }        // 아이템 타입
    public string ItemIconPath { get; protected set; }      // 아이템 아이콘 경로
    public int Quantity { get; protected set; }             // 남은 개수

    public Sprite ItemIcon { get; protected set; }          // 아이템 아이콘

    // ItemData를 통해 아이템의 기본 정보를 초기화
    public virtual void Initialize(ItemData itemData)
    {
        this.ItemName = itemData.itemName;
        this.ItemID = itemData.itemID;
        this.ItemType = itemData.itemType;
        this.ItemDiscription = itemData.itemDiscription;
        this.ItemIconPath = itemData.itemIconPath;
        this.Quantity = itemData.quantity;

        // 아이템 아이콘을 설정
        SetIcon();
    }

    // 공통으로 제공되는 사용/줍기/버리기 메서드
    public virtual void Use()
    {
        Debug.Log($"{ItemName}를 사용하였습니다.");
    }

    public virtual void Drop()
    {
        Debug.Log($"{ItemName}를 드롭하였습니다.");
    }

    public virtual void Pickup()
    {
        Debug.Log($"{ItemName}를 주웠습니다.");
    }

    // 상호작용 인터페이스 구현
    public virtual void Interact()
    {
        Debug.Log($"{ItemName}와 상호작용하였습니다.");
        Use();
        // 상호작용 로직
    }

    // 아이템 아이콘을 설정
    private void SetIcon()
    {
        // 아이템 아이콘을 설정
        if(ItemIconPath == null)
        {
            Debug.LogWarning("아이템 아이콘 경로가 설정되지 않았습니다.");
            return;    
        }

        ItemIcon = GameManager.resourceManager.LoadResource<Sprite>(ItemIconPath);
    }
}
