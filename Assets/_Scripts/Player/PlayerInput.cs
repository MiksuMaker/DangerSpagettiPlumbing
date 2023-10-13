using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Properties
    // Controls
    KeyCode up = KeyCode.W;
    KeyCode left = KeyCode.A;
    KeyCode down = KeyCode.S;
    KeyCode right = KeyCode.D;

    // References
    SnakeController controller;

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
    }

    private void HandleMovementInputs()
    {
        mostRecentMovementInput = Vector2.zero;

        if (Input.GetKey(up)) { mostRecentMovementInput = Vector2.up; }
        if (Input.GetKey(left)) { mostRecentMovementInput = Vector2.left; }
        if (Input.GetKey(down)) { mostRecentMovementInput = Vector2.down; }
        if (Input.GetKey(right)) { mostRecentMovementInput = Vector2.right; }

        // Don't update if no inputs detected
        if (mostRecentMovementInput != Vector2.zero)
        {
            controller.UpdateMovementDirection(mostRecentMovementInput);
        }
    }
    #endregion
}
