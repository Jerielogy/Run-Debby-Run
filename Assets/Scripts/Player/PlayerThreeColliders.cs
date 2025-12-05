using UnityEngine;

public class PlayerThreeColliders : MonoBehaviour
{
    [Header("Colliders")]
    public BoxCollider2D runningCollider;   // Default running collider
    public BoxCollider2D jumpCollider;      // Active when jumping
    public BoxCollider2D crouchCollider;    // Active when crouching

    [Header("Dependencies")]
    public PlayerController playerController; // Reference to your movement script

    void Start()
    {
        // Default state
        runningCollider.enabled = true;
        jumpCollider.enabled = false;
        crouchCollider.enabled = false;
    }

    void Update()
    {
        // Determine state
        bool isCrouching = playerController != null && playerController.isCrouching;
        bool isJumping = playerController != null && !playerController.isGrounded && !isCrouching;

        // Priority: Jump > Crouch > Running
        if (isJumping)
        {
            jumpCollider.enabled = true;
            crouchCollider.enabled = false;
            runningCollider.enabled = false;
        }
        else if (isCrouching)
        {
            crouchCollider.enabled = true;
            jumpCollider.enabled = false;
            runningCollider.enabled = false;
        }
        else
        {
            runningCollider.enabled = true;
            jumpCollider.enabled = false;
            crouchCollider.enabled = false;
        }
    }
}