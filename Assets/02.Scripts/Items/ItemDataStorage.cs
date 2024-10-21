using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// 리스트를 감싸는 구조로 콜렉션 클래스라 생각
public class ItemCollection
{
    public List<ItemData> items;
}

/// <summary>
/// 아이템 저장소 클래스
/// </summary>
public class ItemDataStorage
{
    private readonly string itemInfoPath = "Json/ItemInfo";		// 아이템 정보 저장 경로
    private Dictionary<string, ItemData> _itemDataDictionary;   // 아이템 이름을 키로 하는 Dictionary

    // 생성자
    public ItemDataStorage()
    {
        _itemDataDictionary = new Dictionary<string, ItemData>();
        SetItemsToDictionary();  // 클래스 생성 시 데이터를 로드하여 Dictionary에 저장

        foreach (var item in _itemDataDictionary)
        {
            Debug.Log($"Item added to dictionary: {item.Key} - {item.Value}");
            if(item.Value is MedicalItemData medicalItem)
            {
                Debug.Log($"StatChanges: {medicalItem.statChanges.Count}");
            }
        }
    }

	// @todo : 아이템 정보 저장를 어디에 저장할지 정해야함
    public void SaveItems(List<ItemData> items)
    {
        SaveLoadManager.Save(new ItemCollection { items = items }, itemInfoPath);
    }

    /// <summary>
    /// 아이템 데이터를 Dictionary에 저장하는 메서드
    /// </summary>
    private void SetItemsToDictionary()
    {
        // JSON 데이터를 문자열로 읽어옴
        string jsonData = SaveLoadManager.LoadToString(itemInfoPath);

        // 리스트(ItemCollection)를 역직렬화
        var itemCollection = JsonConvert.DeserializeObject<ItemCollection>(jsonData, new ItemDataConverter());

        // 각 아이템을 Dictionary에 저장
        foreach (var item in itemCollection.items)
        {
            _itemDataDictionary[item.itemID] = item;  // 아이템 이름을 키로 사용
        }
    }

    /// <summary>
    /// 아이템 데이터를 아이템 ID로 검색하는 메서드
    /// </summary>
    public ItemData GetItemDataByID(string itemID)
    {
        if (_itemDataDictionary.TryGetValue(itemID, out var itemData))
        {
            return itemData;
        }
        else
        {
            Debug.LogError($"아이템 {itemID}에 대한 데이터를 찾을 수 없습니다.");
            return null;
        }
    }

    /// <summary>
    /// 아이템 데이터를 List로 반환하는 메서드
    /// </summary>
    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(_itemDataDictionary.Values);
    }

}