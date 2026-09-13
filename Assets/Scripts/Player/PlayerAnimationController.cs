using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetMoving(bool isMoving)
    {
        if (isMoving)
        {
            animator.SetFloat("walkSpeed", 1f);
        }
        else
        {
            animator.SetFloat("walkSpeed", 0f);
        }
    }

    public void SetMining(bool isMining)
    {
        animator.SetBool("isMining", isMining);
    }
}