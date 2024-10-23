using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이어의 기본 설정을 관리하는 클래스
/// </summary>
public class PlayerBasicSettings
{
    // Player Maximum Settings
    public static readonly float maxHealth = 100.0f;        // 플레이어의 최대 체력
    public static readonly float maxMental = 100.0f;        // 플레이어의 최대 스태미나

    // Player Movement Settings
    public static readonly float sitSpeed = 1.0f;           // 앉은 상태 이동 속도
    public static readonly float walkSpeed = 2.0f;          // 걷기 속도
    public static readonly float runSpeed = 4.0f;           // 달리기 속도
    public static readonly float speedChangeRate = 10.0f;   // 이동 속도 변경 비율
    public static readonly float jumpHeight = 0.5f;         // 점프 높이

    // Player Attack Settings
    public static readonly float attackRange = 1.5f;
    public static readonly float attackDamage = 10.0f;
    public static readonly float attackDelay = 1.0f;
}
/// <summary>
/// 플레이어의 상태(체력, 스태미나)를 관리하는 클래스
/// </summary>
public class PlayerStatus
{
    // 플레이어 상태가 추후 더 많은 상태를 다루게 된다면 딕셔너리 구조로 변경하는 것도 고려해볼만함

    public float CurrentHealth { get; private set; }
    public float CurrentMental { get; private set; }
    public float CurrentAttackRange { get; private set; }
    public float CurrentAttackDamage { get; private set; }

    public Action<float> OnHealthChanged;   // 체력이 변경될 때 호출할 이벤트

    public PlayerStatus(){
        CurrentHealth = PlayerBasicSettings.maxHealth; // 초기값을 maxHealth로 설정
        CurrentMental = PlayerBasicSettings.maxMental;
        CurrentAttackDamage = PlayerBasicSettings.attackDamage;
        CurrentAttackRange = PlayerBasicSettings.attackRange;

        OnHealthChanged += (value) => GameManager.uiManager.UpdateUIText(UIConstants.HP, value);
    }

    /// <summary>
    /// Player의 Status를 조절하는 함수
    /// </summary>
    /// <param name="statusType">조절할 Status</param>
    /// <param name="amount">조절 양</param>
    public void AdjustStatus(StatType statusType, float amount)
    {
        switch(statusType)
        {
            case StatType.Health:
                float newHealth = Mathf.Clamp(CurrentHealth + amount, 0, PlayerBasicSettings.maxHealth);
                if(newHealth != CurrentHealth){
                    CurrentHealth = newHealth;
                    OnHealthChanged?.Invoke(CurrentHealth);
                }
                break;
            case StatType.Mental:
                CurrentMental = Mathf.Clamp(CurrentMental + amount, 0, PlayerBasicSettings.maxMental);
                break;
            case StatType.Speed:
                // @TODO:추후 이동속도 감소를 어떻게 처리할지 결정
                break;
        }
    }
}
