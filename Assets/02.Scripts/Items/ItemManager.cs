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
    private Dictionary<ItemType, ItemFactory> _factories = new(); // 아이템 타입별 Factory 저장
    private string _itemPath = "Prefabs/Items/";       // 아이템 프리팹 경로 [추후 수정]

    // 아이템 데이터 딕셔너리 [아이템 ID, 아이템 데이터]
    private Dictionary<string, ItemData> _dataDictionary = new();       
    private readonly string _dataPath = "Json/ItemInfo";                       // 아이템 데이터 경로 [추후 수정]

    // 생성자
    public ItemManager(){
        // 아이템 타입별 Factory 등록
        _factories.Add(ItemType.Medical, new MedicalItemFactory());

        LoadItemsFromJson(_dataPath);

        // // 테스트용 코드
        // for(int i = 0; i < _dataDictionary.Count; i++){
        //     ItemData itemData = _dataDictionary["Item00"+(i+1)];
        //     if(itemData.itemType == ItemType.Medical){
        //         MedicalItemData medicalItemData = (MedicalItemData)itemData;
        //         Debug.Log(medicalItemData.statChanges[0].Stat);
        //     }
        // }
    }

    /// <summary>
    /// 아이템 매니저 초기화 메서드
    /// </summary>
    public void Initialize(string sceneName)
    {
        
    }

    /// <summary>
    /// 아이템 데이터를 JSON 파일로부터 읽어와 딕셔너리에 저장
    /// </summary>
    /// <param name="jsonFilePath">json 파일 경로</param>
    public void LoadItemsFromJson(string jsonFilePath)
    {
        // JSON 파일을 읽어와서 아이템 데이터를 딕셔너리에 저장
        TextAsset jsonDataAsset = GameManager.resourceManager.LoadResource<TextAsset>(jsonFilePath);  
        string jsonData = jsonDataAsset.text;

        // 커스텀 JsonConverter를 사용하여 적절한 타입으로 역직렬화
        Dictionary<string, ItemData> loadedItems = JsonConvert.DeserializeObject<Dictionary<string, ItemData>>(jsonData, new ItemDataConverter());

        foreach (var item in loadedItems)
        {
            _dataDictionary[item.Key] = item.Value;  // 아이템 ID를 키로 사용하여 데이터 저장
        }
    }

    /// <summary>
    /// 아이템 생성 메서드 [ 추후 Object Pooling 적용 ]
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="position"></param>
    public void SpawnItem(string itemID, Vector3 position)
    {
        if (!_dataDictionary.ContainsKey(itemID))
        {
            Debug.LogError($"해당 아이템 ID가 존재하지 않습니다. : {itemID}");
            return;
        }

        ItemData itemData = _dataDictionary[itemID];
        ItemType itemType = itemData.itemType;

        // 아이템 프리팹 로드
        GameObject itemPrefab = GameManager.resourceManager.LoadResource<GameObject>(_itemPath + itemID);

        // 아이템 생성
        ItemBase item = GenerateItemByType(itemType, itemPrefab, itemData);
        if (item != null)
        {
            item.gameObject.transform.position = position;
        }
        else
        {
            Debug.LogError($"아이템 생성에 실패하였습니다. : {itemID}");
        }
    }

    /// <summary>
    /// ItemType에 따라 아이템 Data를 적절한 타입으로 변환하여 아이템 생성
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="itemPrefab"></param>
    /// <param name="itemData"></param>
    /// <returns></returns>
    private ItemBase GenerateItemByType(ItemType itemType, GameObject itemPrefab, ItemData itemData)
    {
        switch(itemType)
        {
            case ItemType.Medical:
                return CreateTypedItem<MedicalItemData>(itemPrefab, itemData);
            // 다른 ItemType들에 대한 처리가 추가될 수 있음
            default:
                Debug.LogError($"해당 타입은 존재하지 않습니다. : {itemType}");
                return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">생성될 아이템 타입 Data 클래스</typeparam>
    /// <param name="itemPrefab"></param>
    /// <param name="itemData"></param>
    /// <returns></returns>
    private ItemBase CreateTypedItem<T>(GameObject itemPrefab, ItemData itemData) where T : ItemData
    {   
        // ItemData를 T 타입으로 캐스팅 가능 여부 확인
        if (itemData is not T typedData)
        {
            Debug.LogError($"{typeof(T).Name}에 대한 캐스팅 실패");
            return null;
        }

        return _factories[typedData.itemType].CreateItem(itemPrefab, typedData);
    }


    // SystemManager를 이용한 Save & Load
    
}

// ItemDataConverter 정의 [ 추후 최적화 조금 더 고민 ]
public class ItemDataConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // / ItemData 또는 이를 상속하는 클래스만 변환 가능
        return typeof(ItemData).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        // JSON 토큰을 파싱
        JObject jsonObject = JObject.Load(reader);

        // itemType 필드를 기반으로 어떤 클래스인지 결정
        var itemTypeToken = jsonObject["itemType"];
        if (itemTypeToken == null)
        {
            throw new Exception("itemType is missing in the JSON data.");
        }

        // 문자열 대신 Enum 사용
        if (!Enum.TryParse(itemTypeToken.ToString(), out ItemType itemType))
        {
            throw new Exception($"Invalid itemType: {itemTypeToken}");
        }
        
        ItemData item;
        
        switch (itemType)
        {
            case ItemType.Medical:
                item = new MedicalItemData();
                break;
            default:
                item = new ItemData();  // 기본 ItemData로 역직렬화
                break;
        }

        // 공통 필드 역직렬화
        serializer.Populate(jsonObject.CreateReader(), item);

        return item;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // 직렬화는 필요 없으므로 구현하지 않음
        throw new NotImplementedException();
    }
}