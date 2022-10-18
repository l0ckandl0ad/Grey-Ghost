using UnityEngine;

public abstract class IconDirectionForUnitBase : MonoBehaviour
{
    protected Animator animator;

    void Awake()
    {
        CacheReferences();
    }

    protected virtual void CacheReferences()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Takes movement vector from outside source, ie player input or navmesh agent velocity.
    /// </summary>
    /// <returns></returns>
    protected abstract Vector3 CheckMovementVector();

    /// <summary>
    /// This function checks if player moves left or right based on its velocity vector and applies the changes accordingly.
    /// </summary>
    /// <returns></returns>
    protected virtual void CheckMovementDirection(Vector3 movement)
    {
        if (movement.x > 0)
        {
            MoveRight();
        }
        else if (movement.x < 0)
        {
            MoveLeft();
        }
    }

    protected virtual void MoveLeft()
    {
        animator.SetTrigger("MoveLeft");
    }
    protected virtual void MoveRight()
    {
        animator.SetTrigger("MoveRight");
    }

    protected void Update()
    {
        CheckMovementDirection(CheckMovementVector());
    }
}
