using System;
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
    public string itemID;                   // 아이템 ID
    public string itemName;                 // 아이템 이름
    public string itemDescription;          // 아이템 설명
    public ItemType itemType;               // 아이템 타입
    public string itemIconPath;             // 아이템 아이콘 경로
    public int quantity;                    // 아이템 개수
}

public class MedicalItemData : ItemData
{
    public List<StatChange> statChanges;    // 속성 변화 목록
    public int repeatCount;                 // 반복 사용 가능 횟수
}