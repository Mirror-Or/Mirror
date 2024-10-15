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
    
    public void SetAnimationFloat(string parameter, float value)
    {
        _fpsAnimator?.SetFloat(parameter, value);
        _thirdPersonAnimator?.SetFloat(parameter, value);
    }

    public void SetAnimationBool(string parameter, bool value)
    {
        _fpsAnimator?.SetBool(parameter, value);
        _thirdPersonAnimator?.SetBool(parameter, value);
    }

    public void TriggerAnimation(string parameter)
    {
        _fpsAnimator?.SetTrigger(parameter);
        _thirdPersonAnimator?.SetTrigger(parameter);
    }
}
