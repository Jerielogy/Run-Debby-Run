using UnityEngine;
using UnityEngine.EventSystems; // Required for touching buttons

public class OnScreenControls : MonoBehaviour
{
    private PlayerController player;
    private bool isHoldingCrouch = false;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        // --- GATEKEEPER CHECK ---
        // 0 = Keyboard, 1 = Voice, 2 = Joystick
        int controlScheme = PlayerPrefs.GetInt("ControlScheme", 0);

        // If the setting is NOT 2 (Joystick), hide these buttons and stop running.
        if (controlScheme != 2)
        {
            gameObject.SetActive(false); // Turn off the whole panel
        }
    }

    void Update()
    {
        // Continuous Crouching (while holding the button)
        if (isHoldingCrouch && player != null)
        {
            player.Crouch();
        }
    }

    // --- BUTTON FUNCTIONS ---

    // Connect this to the Jump Button (OnClick)
    public void OnJumpButtonPress()
    {
        if (player != null) player.Jump();
    }

    // Connect this to Crouch Button (Event Trigger: Pointer Down)
    public void StartCrouching()
    {
        isHoldingCrouch = true;
    }

    // Connect this to Crouch Button (Event Trigger: Pointer Up)
    public void StopCrouching()
    {
        isHoldingCrouch = false;
        if (player != null) player.StopCrouch();
    }
}