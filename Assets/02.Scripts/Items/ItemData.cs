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
    public string itemID;
    public string itemName;
    public string itemDescription;
    public ItemType itemType;
    public string itemIconPath;
    public int quantity;
}

public class MedicalItemData : ItemData
{
    public List<StatChange> statChanges;
}