using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory
{   
    private string _itemPrefabPath = "Prefabs/Items/";
    private Dictionary<string, GameObject> _cachedPrefabs = new(); // 프리팹 캐싱
    
    /// <summary>
    /// 아이템 데이터를 받아 해당 아이템을 생성하는 메서드
    /// </summary>
    /// <param name="itemData"></param>
    public ItemBase CreateItem(ItemData itemData)
    {
        GameObject itemPrefab = GetItemPrefab(itemData.itemID);
        if (itemPrefab == null)
        {
            Debug.LogError($"프리팹을 찾을 수 없습니다: {itemData.itemType}");
            return null;
        }

        // 프리팹을 인스턴스화
        GameObject itemObject = GameObject.Instantiate(itemPrefab);
        switch (itemData.itemType)
        {
            case ItemType.Medical:
                // 게임 오브젝트에 MedicalItem 컴포넌트 추가
                MedicalItem a = itemObject.AddComponent<MedicalItem>();
                
                // itemData를 컴포넌트에 초기화
                a.Initialize(itemData as MedicalItemData);
                
                Debug.Log($"{a.StatChanges.Count} {a}");
                return a;
            case ItemType.Quest:
            case ItemType.Functional:
            case ItemType.Weapon:
                return null;
            default:
                throw new System.Exception($"해당 타입과 매칭되는 타입이 없습니다. : {itemData.itemType}");
        }
    }

    // 프리팹을 캐싱하여 가져오는 메서드
    private GameObject GetItemPrefab(string itemID)
    {
        if(_cachedPrefabs.ContainsKey(itemID))
        {
            return _cachedPrefabs[itemID];
        }

        string prefabPath = $"{_itemPrefabPath}{itemID}";
        GameObject prefab = GameManager.resourceManager.LoadResource<GameObject>(prefabPath);

        if (prefab != null)
        {
            _cachedPrefabs[itemID] = prefab; // 프리팹 캐싱
        }

        return prefab;
    }
}