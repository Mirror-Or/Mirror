using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

/// <summary>
/// 스텟 변화 아이템 클래스
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum StatType
{
    Health,
    Mental,
    Speed
}

/// <summary>
/// 스텟 변화 아이템 클래스
/// </summary>
public class StatChange
{
    public StatType stat;
    public int changeAmount;
    public float duration;
    public int repeatCount;
}

/// <summary>
/// 회복 아이템 클래스
/// </summary>
public class MedicalItem : ItemBase
{
    public List<StatChange> StatChanges {get; protected set;}    // 속성 변화 목록

    private  PlayerStatus _playerStatus; // 플레이어 상태 참조

    public void Initialize(MedicalItemData itemData)
    {
        base.Initialize(itemData);
        Debug.Log($"{itemData.itemName}");
        Debug.Log($"{itemData.itemDescription}");
        Debug.Log($"{itemData.itemIconPath}");
        Debug.Log($"{itemData.quantity}");
        Debug.Log($"{itemData.statChanges.Count}");

        StatChanges = itemData.statChanges;

        _playerStatus = GameManager.playerManager.GetPlayerStatus();
    }

    public override void Interact()
    {
        base.Interact();

        Use();
        // 아이템 사용에 따른 플레이어 스테이터스 변화 필요
    }

    // Use 메서드에서 상태 변화 처리
    public override void Use()
    {
        if(this.Quantity <= 0)
        {
            Debug.Log("아이템 개수가 부족합니다.");
            return;
        }
        
        base.Use();

        foreach (var statChange in StatChanges)
        {
            if (statChange.duration == 0) 
            {
                // 즉발성 효과 적용
                ApplyImmediateEffect(statChange);
            }
            else
            {
                // 지속성 효과 적용 (코루틴으로 반복 처리)
                StartCoroutine(ApplyOverTime(statChange));
            }
        }

        // 아이템 사용후 아이템 개수 감소
        this.Quantity--;
        Debug.Log($"{ItemName}의 남은 개수 : {Quantity}");
    }

    // 즉발성 효과 적용
    private void ApplyImmediateEffect(StatChange statChange)
    {
        switch (statChange.stat)
        {
            case StatType.Health:
                Debug.Log($"{statChange.changeAmount}만큼 체력이 즉시 회복되었습니다.");
                // 체력 회복 로직
                ApplyHealthChange(statChange);
                break;
            case StatType.Mental:
                Debug.Log($"{statChange.changeAmount}만큼 정신력이 즉시 변경되었습니다.");
                // 정신력 변화 로직
                ApplyMentalChange(statChange);
                break;
            case StatType.Speed:
                Debug.Log($"이동속도가 {statChange.changeAmount}만큼 즉시 변경되었습니다.");
                // 이동속도 변화 로직
                ApplySpeedChange(statChange);
                break;
        }
    }

    // 지속성 효과 적용 (코루틴)
    private IEnumerator ApplyOverTime(StatChange statChange)
    {
        for (int i = 0; i < statChange.repeatCount; i++)
        {
            switch (statChange.stat)
            {
                case StatType.Health:
                    Debug.Log($"{statChange.changeAmount}만큼 체력이 {statChange.duration}초에 걸쳐 회복됩니다.");
                    // 체력 회복 로직
                    ApplyHealthChange(statChange);
                    break;
                case StatType.Mental:
                    Debug.Log($"{statChange.changeAmount}만큼 정신력이 {statChange.duration}초에 걸쳐 변경됩니다.");
                    // 정신력 변화 로직
                    ApplyMentalChange(statChange);
                    break;
                case StatType.Speed:
                    Debug.Log($"이동속도가 {statChange.changeAmount}만큼 {statChange.duration}초에 걸쳐 변경됩니다.");
                    // 이동속도 변화 로직
                    ApplySpeedChange(statChange);
                    break;
            }

            yield return new WaitForSeconds(statChange.duration); // 반복 사이 시간 대기
        }
    }

    // @TODO: 추후 현재 스테이터스 상태가 가득찬 상태일 때에도 아이템이 사용되는지 확인 필요

    // 체력 변화 로직
    private void ApplyHealthChange(StatChange change)
    {
        // 플레이어의 체력에 changeAmount만큼 변화를 줌
        Debug.Log("Applying health change: " + change.changeAmount);

        float currentHealth = _playerStatus.CurrentHealth;
        _playerStatus.AdjustStatus(StatType.Health, change.changeAmount);

        Debug.Log($"기존 체력 : {currentHealth} / 현재 체력 : {_playerStatus.CurrentHealth}");
    }

    // 정신력 변화 로직
    private void ApplyMentalChange(StatChange change)
    {
        Debug.Log("Applying mental change: " + change.changeAmount);

        float currentMental = _playerStatus.CurrentMental;
        _playerStatus.AdjustStatus(StatType.Mental, change.changeAmount);

        Debug.Log($"기존 체력 : {currentMental}  현재 정신력 : {_playerStatus.CurrentMental}");
    }

    // 속도 변화 로직
    private void ApplySpeedChange(StatChange change)
    {
        Debug.Log("Applying speed change: " + change.changeAmount);
        _playerStatus.AdjustStatus(StatType.Speed, change.changeAmount);
    }
}
