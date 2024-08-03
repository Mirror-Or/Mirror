using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory_Container
{
    [SerializeField]
    private List<GameObject> Container;

    public void Initialize()
    {
        Container = new List<GameObject>(3);
        Debug.Log("Create Container");
    }

    public void Add_Item(GameObject _obj, string _str)
    {
        GameObject Item = Container.Find(x => x.GetComponent<BaseItem>().itemData.name == _str);

        if (null == Item)
            Container.Add(_obj); // ���� �κ��丮�� ���� ���ο� ������ -> �����̳ʿ� �߰�
        else
            Item.GetComponent<BaseItem>().itemData.count++; // ���� �κ��丮�� �ִ� ������ -> ���� �߰�
    }

    public GameObject Get_Item(string _name)
    {
        GameObject Item = Container.Find(x => x.GetComponent<BaseItem>().itemData.name == _name);
        if (null != Item)
            return Item;

        //for (int i = 0; i < Container.Count; i++)
        //{
        //    if (Container[i].GetComponent<BaseItem>().itemData.name == _name)
        //        return Container[i];
        //}

        Debug.Log("Item not found");
        return null;
    }

    public void Use_Item(int _idx)
    {
        if (_idx >= Container.Count)
        {
            Debug.Log("index is bigger then Container Size");
            return;
        }

        Container[_idx].GetComponent<BaseItem>().UseItem();

        if (--Container[_idx].GetComponent<BaseItem>().itemData.count <= 0)
            Remove_Item(_idx);
    }

    public void Remove_Item(int _idx)
    {
        //���� ���� ���� �߰�

        Container.Remove(Container[_idx]);
    }
}