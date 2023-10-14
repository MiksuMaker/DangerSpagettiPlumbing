using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMover : MonoBehaviour
{
    #region Properties
    // References
    SnakeBodyHandler body;

    Vector2 ORIGINALMoveDirection = Vector2.right;
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
        // Check that nothing is impeding movement
        if (body.interruptableMovementInProgress) { return; }
        if (TileManager.Instance.IsThereTileAt(body.GetCurrentCoordinates() + currentMoveDirection)) { return; }

        // Move forwards
        body.MoveSnakeBody(currentMoveDirection);

        DoEdibleCheck();

        body.CalculateFallDistance();
    }

    public void TryUpdateMovementDirection(Vector2 newDirection)
    {
        // Check if movement direction is valid
        if (TileManager.Instance.IsThereTileAt(body.GetCurrentCoordinates() + newDirection))
        {
            // Blocked movement!
            return;
        }

        if (body.IsThereSnakePieceAt(body.GetCurrentCoordinates() + newDirection))
        {
            // Snake piece is on the way
            return;
        }

        currentMoveDirection = newDirection;

        // Manual movement
        //DoMove();


    }

    private void DoEdibleCheck()
    {
        Vector2 curPos = body.GetCurrentCoordinates();

        // Check if on top of any edibles
        if (CheckPosForEdibles(curPos))
        {
            // Eat them
            EdibleManager.Instance.EatEdibleAtPos(curPos);
        }

        // Check for edibles in front of the snake
        if (CheckPosForEdibles(curPos + currentMoveDirection * 2f))
        { body.openMouth = true; } // Open mouth
        else
        { body.openMouth = false; } // Close mouth
    }

    private bool CheckPosForEdibles(Vector2 checkPos)
    {
        return EdibleManager.Instance.CheckPositionForEdibles(checkPos);
    }

    public void ResetSnake()
    {
        currentMoveDirection = ORIGINALMoveDirection;
        body.ReturnToStartPositions();
    }
    #endregion
}
