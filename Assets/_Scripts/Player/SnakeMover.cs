using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMover : MonoBehaviour
{
    #region Properties
    // References
    SnakeBodyHandler body;

    Vector2 currentMoveDirection = Vector2.right;
    #endregion

    #region Setup
    private void Awake()
    {
        body = GetComponent<SnakeBodyHandler>();
    }

    private void Start()
    {
        // Subscribe to delegates
        FindObjectOfType<PlayerInput>().resetGame += ResetSnake;
        MovementManager.Instance.movementTick += DoMove;
    }
    #endregion

    #region Functions
    public void DoMove()
    {
        // Move forwards
        if (body.interruptableMovementInProgress) { return; }

        body.MoveSnakeBody(currentMoveDirection);

        body.CalculateFallDistance();
    }

    public void TryUpdateMovementDirection(Vector2 newDirection)
    {
        // Check if movement direction is valid
        if (TileManager.Instance.IsThereTileAt(body.GetCurrentCoordinates() 
                                                    + newDirection))
        {
            // Blocked movement!
            return;
        }

        if (body.IsThereSnakePieceAt(body.GetCurrentCoordinates() 
                                                    + newDirection))
        {
            // Snake piece is on the way
            return;
        }

        currentMoveDirection = newDirection;

        // Manual movement
        DoMove();
    }

    public void ResetSnake()
    {
        body.ReturnToStartPositions();
    }
    #endregion
}
