using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Numerics;
using UHFPS.Runtime;
using UnityEngine;

[System.Serializable]
public class Inventory_Manager : MonoBehaviour
{
    private static Inventory_Manager instance;

    [SerializeField]
    private Inventory_Container[] Mirror_Inventory;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        if(null == instance )
        {
            instance = this;

            Mirror_Inventory = new Inventory_Container[(int)ItemType.END];
            for (int i = 0; i < Mirror_Inventory.Length; i++)
            {
                Mirror_Inventory[i] = new Inventory_Container();
                Mirror_Inventory[i].Initialize();
            }

            DontDestroyOnLoad( this.gameObject );
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Inventory_Manager Instance
    {
        get
        {
            if ( null == instance )
            {
                return null;
            }
            return instance;
        }
    }





    // �κ��丮 ���
    public bool Add_Item(GameObject _obj)
    {
        if (null == _obj) // GameObject�� null���� üũ
        {
            Debug.Log("NULL Object");
            return false;
        }

        if (_obj.GetComponent<BaseItem>() == null) // GameObject�� Item���� üũ
        {
            Debug.Log("GameObject is not BaseItem");
            return false;
        }

        ItemType type = _obj.GetComponent<BaseItem>().itemData.type;
        string str = _obj.GetComponent<BaseItem>().itemData.name;

        Mirror_Inventory[(int)type].Add_Item( _obj, str);
        return true;
    }

    //�κ��丮 ��ܿ� ����� �ְ�, �����ý� �ٵ��� �κ��丮�� ����(4 x 10), Ư�� ��ġ�� �������� ����ϴ� ��Ȳ���� ����
    public void Use_Item(ItemType _type, int x, int y)
    {
        //�κ��丮�� ����, ����ũ�⿡ ���� �ٸ��� ������ ����.
        Mirror_Inventory[(int)_type].Use_Item(4 * y + x);
    }

    GameObject Get_Item(string _name, ItemType _type)
    {
        return Mirror_Inventory[(int)_type].Get_Item(_name);
    }
}
