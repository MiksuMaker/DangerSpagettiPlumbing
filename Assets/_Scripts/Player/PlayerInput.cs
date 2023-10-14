using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Properties
    // References
    SnakeController controller;

    // Events
    public delegate void ResetGame();
    public event ResetGame resetGame;

    // Controls
    KeyCode up = KeyCode.W;
    KeyCode left = KeyCode.A;
    KeyCode down = KeyCode.S;
    KeyCode right = KeyCode.D;

    KeyCode restart = KeyCode.R;
    // Inputs
    Vector2 mostRecentMovementInput;
    #endregion

    #region Setup
    private void Start()
    {
        // Find Controller
        controller = FindObjectOfType<SnakeController>();
    }
    #endregion

    #region Inputs
    private void Update()
    {
        // Listen for inputs
        HandleMovementInputs();
        HandleDebugControls();
    }

    private void HandleMovementInputs()
    {
        mostRecentMovementInput = Vector2.zero;

        if (Input.GetKeyDown(up)) { mostRecentMovementInput = Vector2.up; }
        if (Input.GetKeyDown(left)) { mostRecentMovementInput = Vector2.left; }
        if (Input.GetKeyDown(down)) { mostRecentMovementInput = Vector2.down; }
        if (Input.GetKeyDown(right)) { mostRecentMovementInput = Vector2.right; }

        // Don't update if no inputs detected
        if (mostRecentMovementInput != Vector2.zero)
        {
            controller.UpdateMovementDirection(mostRecentMovementInput);
        }
    }

    private void HandleDebugControls()
    {
        if (Input.GetKeyDown(restart))
        {
            resetGame?.Invoke();
        }
    }
    #endregion
}
