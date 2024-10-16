using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 플레이어 애니메이션 이벤트 핸들러
/// </summary>
public class PlayerAnimationEventHandler : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip landingAudioClip;                     // 착지 사운드
    [SerializeField] private AudioClip[] footstepAudioClips;                 // 발소리 사운드
    [SerializeField, Range(0, 1)] private float footstepAudioVolume = 0.5f;  // 발소리 사운드 볼륨
    [SerializeField] private AudioClip attackAudioClip;                      // 공격 사운드
    
    [SerializeField] private CharacterController _characterController;       //  캐릭터 컨트롤러

    #region Animation Events
    /// <summary>
    /// 발소리 이벤트
    /// </summary>
    public void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && footstepAudioClips.Length > 0)
        {
            var index = Random.Range(0, footstepAudioClips.Length);
            var clip = footstepAudioClips[index];
            if (clip != null)
            {
                GameManager.audioManager.PlaySoundEffect(clip, transform.TransformPoint(_characterController.center), footstepAudioVolume);
            }
            else
            {
                Debug.LogWarning("걷기 사운드 클립이 없습니다.");
            }
        }
    }

    /// <summary>
    /// 착지 이벤트
    /// </summary>
    /// <param name="animationEvent"></param>
    public void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && landingAudioClip != null)
        {
            GameManager.audioManager.PlaySoundEffect(landingAudioClip, transform.TransformPoint(_characterController.center), footstepAudioVolume);
        }else{
            Debug.Log("착지 사운드 클립이 없습니다.");
        }
    }
    #endregion
}
