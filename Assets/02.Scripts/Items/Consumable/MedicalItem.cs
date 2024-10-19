using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MedicalItemData : ItemData
{
    public List<StatChange> statChanges;
}

/// <summary>
/// 스텟 변화 아이템 클래스
/// </summary>
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
    public StatType Stat { get; private set; }
    public float ChangeAmount { get; private set; } // 변화량 (양수: 증가, 음수: 감소)
    public float Duration { get; private set; }     // 지속 시간 (0이면 즉발성)
    public int RepeatCount { get; private set; }    // 몇 번 반복할지

    public StatChange(StatType stat, float changeAmount, float duration = 0, int repeatCount = 1)
    {
        Stat = stat;
        ChangeAmount = changeAmount;
        Duration = duration;
        RepeatCount = repeatCount;
    }
}

/// <summary>
/// 회복 아이템 클래스
/// </summary>
public class MedicalItem : ConsumableItem
{
    private List<StatChange> _statChanges;      // 속성 변화 목록

    // MedicalItem 전용 초기화
    public override void Initialize(ItemData itemData)
    {
        base.Initialize(itemData);  // 공통 속성 초기화
        _statChanges = ((MedicalItemData)itemData).statChanges;
    }

    // Use 메서드에서 상태 변화 처리
    public override void Use()
    {
        base.Use();

        foreach (var statChange in _statChanges)
        {
            if (statChange.Duration == 0) 
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
    }

    // 즉발성 효과 적용
    private void ApplyImmediateEffect(StatChange statChange)
    {
        switch (statChange.Stat)
        {
            case StatType.Health:
                Debug.Log($"{statChange.ChangeAmount}만큼 체력이 즉시 회복되었습니다.");
                // 체력 회복 로직
                break;
            case StatType.Mental:
                Debug.Log($"{statChange.ChangeAmount}만큼 정신력이 즉시 변경되었습니다.");
                // 정신력 변화 로직
                break;
            case StatType.Speed:
                Debug.Log($"이동속도가 {statChange.ChangeAmount}만큼 즉시 변경되었습니다.");
                // 이동속도 변화 로직
                break;
        }
    }

    // 지속성 효과 적용 (코루틴)
    private IEnumerator ApplyOverTime(StatChange statChange)
    {
        for (int i = 0; i < statChange.RepeatCount; i++)
        {
            switch (statChange.Stat)
            {
                case StatType.Health:
                    Debug.Log($"{statChange.ChangeAmount}만큼 체력이 {statChange.Duration}초에 걸쳐 회복됩니다.");
                    // 체력 회복 로직
                    break;
                case StatType.Mental:
                    Debug.Log($"{statChange.ChangeAmount}만큼 정신력이 {statChange.Duration}초에 걸쳐 변경됩니다.");
                    // 정신력 변화 로직
                    break;
                case StatType.Speed:
                    Debug.Log($"이동속도가 {statChange.ChangeAmount}만큼 {statChange.Duration}초에 걸쳐 변경됩니다.");
                    // 이동속도 변화 로직
                    break;
            }

            yield return new WaitForSeconds(statChange.Duration); // 반복 사이 시간 대기
        }
    }
}
