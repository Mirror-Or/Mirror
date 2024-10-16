using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Player의 입력처리를 담당하는 클래스
/// </summary>
public class PlayerInputAction : IInputActionStrategy
{
    // player input values
    public Vector2 move;
    public Vector2 look;
    public Vector2 mousePosition;

    private const int QUICK_SLOT_COUNT = 4;
    public bool[] quickSlots = new bool[QUICK_SLOT_COUNT];
    public bool isChoiceQuickSlot = false;

    public bool jump = false;
    public bool sprint = false;
    public bool isInventoryVisible = false;
    public bool isQuickSlotVisible = false;
    public bool isInteractable = false;
    public bool isUseItem = false;
    public bool isTransferItem = false;
    public bool isFire = false;
    public bool isSit = false;

    // 마우스 커서 제어
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    // IInputActionStrategy 인터페이스 메서드
    public void BindInputActions(InputActionMap map) => GameManager.inputManager.BindAllActions(map.name, this);
    
    public void OnMovement(InputAction.CallbackContext context) => move = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) => look = cursorInputForLook ? context.ReadValue<Vector2>() : Vector2.zero;
    public void OnMousePosition(InputAction.CallbackContext context) => mousePosition = context.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext context) => jump = context.ReadValueAsButton();
    public void OnSprint(InputAction.CallbackContext context) => sprint = context.ReadValueAsButton();
    public void OnInventory(InputAction.CallbackContext context) => isInventoryVisible = context.ReadValueAsButton();
    public void OnInteraction(InputAction.CallbackContext context) => isInteractable = context.ReadValueAsButton();
    public void OnUseItem(InputAction.CallbackContext context) => isUseItem = context.ReadValueAsButton();
    public void OnTransferItem(InputAction.CallbackContext context) => isTransferItem = context.ReadValueAsButton();
    public void OnFire(InputAction.CallbackContext context) => isFire = context.ReadValueAsButton();
    public void OnShowQuickSlot(InputAction.CallbackContext context) => isQuickSlotVisible = context.performed;
    public void OnSit(InputAction.CallbackContext context) => isSit = context.ReadValueAsButton();

    /// <summary>
    /// 퀵슬롯을 선택하는 함수
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="context"></param>
    private void OnQuickSlot(int slotIndex, InputAction.CallbackContext context)
    {
        if (slotIndex < 0 || slotIndex >= quickSlots.Length)
        {
            Debug.LogError("슬롯 인덱스가 알맞지 않습니다. : " + slotIndex);
            return;
        }

        // 기존 선택된 슬롯을 비활성화하고 새로 선택된 슬롯만 활성화
        if (!quickSlots[slotIndex])
        {
            Array.Fill(quickSlots, false);
            quickSlots[slotIndex] = context.ReadValueAsButton();
            isChoiceQuickSlot = quickSlots[slotIndex];
        }
    }

    public void OnQuickSlot1(InputAction.CallbackContext context) => OnQuickSlot(0, context);
    public void OnQuickSlot2(InputAction.CallbackContext context) => OnQuickSlot(1, context);
    public void OnQuickSlot3(InputAction.CallbackContext context) => OnQuickSlot(2, context);
    public void OnQuickSlot4(InputAction.CallbackContext context) => OnQuickSlot(3, context);
}
