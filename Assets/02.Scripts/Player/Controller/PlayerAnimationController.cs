using UnityEngine;

/// <summary>
/// 애니메이터 매개변수를 관리하는 클래스
/// </summary>
public class AnimatorParameters
{
	[Header("Player Animation IDs")]
    public static readonly int AnimIDAttack 		= Animator.StringToHash("Attack");
    public static readonly int AnimIDSit 			= Animator.StringToHash("Sit");

    public static readonly string Speed = "Speed";
    public static readonly string IsGrounded = "Grounded";
    public static readonly string IsJumping = "Jump";
    public static readonly string IsInFreeFall = "FreeFall";
    public static readonly string IsAttacking = "Attack";
    public static readonly string IsSitting = "Sit";

}


/// <summary>
/// 플레이어 애니메이션 컨트롤러
/// </summary>
public class PlayerAnimationController
{
    private Animator _fpsAnimator;          // 1인칭 애니메이터
    private Animator _thirdPersonAnimator;  // 3인칭 애니메이터

    public PlayerAnimationController(Animator fpsAnimator, Animator thirdPersonAnimator)
    {
        _fpsAnimator = fpsAnimator;
        _thirdPersonAnimator = thirdPersonAnimator;
    }
    
        /// <summary>
        /// 애니메이션 float 값을 설정하는 함수
        /// </summary>
        /// <param name="parameter">파라미터 명</param>
        /// <param name="value">float value</param>
    public void SetAnimationFloat(string parameter, float value)
    {
        _fpsAnimator?.SetFloat(parameter, value);
        _thirdPersonAnimator?.SetFloat(parameter, value);
    }

    /// <summary>
    /// 애니메이션 bool 값을 설정하는 함수
    /// </summary>
    /// <param name="parameter">파라미터 명</param>
    /// <param name="value">bool value</param>
    public void SetAnimationBool(string parameter, bool value)
    {
        _fpsAnimator?.SetBool(parameter, value);
        _thirdPersonAnimator?.SetBool(parameter, value);
    }

    /// <summary>
    /// 애니메이션 트리거를 실행하는 함수
    /// </summary>
    /// <param name="parameter">파라미터명</param>
    public void TriggerAnimation(string parameter)
    {
        _fpsAnimator?.SetTrigger(parameter);
        _thirdPersonAnimator?.SetTrigger(parameter);
    }
}
