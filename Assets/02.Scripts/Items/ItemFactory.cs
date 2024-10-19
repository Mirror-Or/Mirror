using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemFactory
{
    public abstract ItemBase CreateItem(GameObject itemPrefab, ItemData itemData);
}

public class MedicalItemFactory : ItemFactory
{
    public override ItemBase CreateItem(GameObject itemPrefab, ItemData itemData)
    {
        // 추후 Object Pooling을 이용한 생성으로 변경 가능
        GameObject newItem = Object.Instantiate(itemPrefab);            // 프리팹 인스턴스 생성
        MedicalItem medicalItem = newItem.AddComponent<MedicalItem>();  // 컴포넌트 동적 추가 
        medicalItem.Initialize((MedicalItemData)itemData);              // 데이터 초기화
        return medicalItem;
    }
}