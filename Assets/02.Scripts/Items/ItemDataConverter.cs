using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// ItemData를 상속하는 클래스를 변환하는 컨버터 (Custom Converter)
/// </summary>
public class ItemDataConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // ItemData 또는 이를 상속하는 클래스만 변환 가능
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
            // JSON 구조를 디버그로 출력
            Debug.LogError($"itemType 필드가 없습니다. JSON 데이터: {jsonObject}");
            throw new Exception("itemType 필드가 없습니다.");
        }

        // 문자열 대신 Enum 사용
        if (!Enum.TryParse(itemTypeToken.ToString(), out ItemType itemType))
        {
            throw new Exception($"itemType: {itemTypeToken}");
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