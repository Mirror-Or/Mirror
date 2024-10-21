using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// 전체 아이템(소모성, 무기 등)을 관리하는 클래스 
/// </summary>
public class ItemManager : IManager
{
    private ItemDataStorage _itemDataStorage = new();       // 아이템 저장소
    public List<ItemData> items = new();                    // 아이템 목록
    private ItemFactory _itemFactory = new();               // 아이템 팩토리

    public GameObject SpawnItem(string itemID, Vector3 position)
    {
        // 아이템 데이터를 불러옴
        ItemData itemData = _itemDataStorage.GetItemDataByID(itemID);

        if (itemData == null)
        {
            Debug.LogError($"아이템 {itemID}에 대한 데이터를 찾을 수 없습니다.");
            return null;
        }

        // ItemFactory를 통해 아이템 생성
        var item = _itemFactory.CreateItem(itemData);
        if (item == null)
        {
            Debug.LogError("아이템을 생성하는 데 실패했습니다.");
            return null;
        }

        // 아이템의 게임 오브젝트를 스폰 위치에 배치
        GameObject itemInstance = item.gameObject;
        itemInstance.transform.position = position;

        return itemInstance;
    }

    /// <summary>
    /// 아이템 매니저 초기화 메서드
    /// </summary>
    public void Initialize(string sceneName)
    {
        
    }

}