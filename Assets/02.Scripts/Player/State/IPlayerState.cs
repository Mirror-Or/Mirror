using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    void EnterState(PlayerFSM playerFSM);   // 상태 진입 시 호출
    void UpdateState(PlayerFSM playerFSM);  // 매 프레임 상태를 업데이트
    void ExitState(PlayerFSM playerFSM);    // 상태 종료 시 호출
}
