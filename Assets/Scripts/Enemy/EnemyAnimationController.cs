using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;


    public void SetMoving(bool isMoving)
    {
        animator.SetFloat(
            "walkSpeed",
            isMoving ? 1f : 0f
        );
    }


    public void SetAttacking(bool isAttacking)
    {
        animator.SetBool(
            "isAttacking",
            isAttacking
        );
    }
}