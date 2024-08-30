using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DragSlot : MonoBehaviour
{
    static public UI_DragSlot instance;
    public bool IsClear = true;         //���Կ��� OnDropȣ��� ��ȿ�� �׼����� �Ǵ��ϱ� ���� ���
    private UI_Slot_bls DragSlot;       //�巡�� ���۽� �������� ����
    private IInventoryItem Item;        //�巡�� ���۽� �����ϴ� ������ ������

    [SerializeField] private Image ItemIcon;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Clear()
    {
        Set_Alpha(0f);      //�������

        DragSlot = null;
        Item = null;
        ItemIcon.sprite = null;

        IsClear = true;
    }

    public void StartDrag(PointerEventData _eventData, UI_Slot_bls _slot)   //���Կ��� �巡�� ���۽� ȣ��
    {
        DragSlot = _slot;                                                   //���� ���� ������Ʈ
        Item = _slot.Get_Item();                                            //������ ���� ������Ʈ
        transform.position = _eventData.position;                           //�巡�� ������ҷ� ������ ����
        IsClear = false;                                                    //OnDrop�� ��ȿ�� ����üũ��

        Set_Alpha(1f);                                                      //alpha�� 1�� �ٲ��� ������� ����

        Update_DragSlot();
    }

    public void EndDrag()   //�巡�װ� ���� ������ ȣ��
    {
        Clear();            //�巡�� ���� �ʱ�ȭ
    }

    private void Update_DragSlot()
    {
        if(Item != null)
            ItemIcon.sprite = Item.Icon;
    }

    private void Set_Alpha(float _alpha)
    {
        Color color = ItemIcon.color;
        color.a = _alpha;
        ItemIcon.color = color;
    }

    public UI_Slot_bls Get_Slot()
    {
        return DragSlot;
    }

}
