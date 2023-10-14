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

    Vector2 bufferMoveDirection = Vector2.zero;
    bool freshBuffer = false;
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
        HandleBufferMovement();

        Vector2 curPos = body.GetCurrentCoordinates();

        // Check that nothing is impeding movement
        if (body.interruptableMovementInProgress) { return; }
        if (TileManager.Instance.IsThereTileAt(curPos + currentMoveDirection)) { return; }
        if (body.IsThereSnakePieceAt(curPos + currentMoveDirection)) { return; }

        // Move forwards
        body.MoveSnakeBody(currentMoveDirection);

        DoEdibleCheck();

        body.CalculateFallDistance();
    }

    private void HandleBufferMovement()
    {
        // Check if there is buffer move input
        if (bufferMoveDirection != Vector2.zero)
        {
            // Check if it was made just now
            if (freshBuffer)
            {
                // If it is too fresh, wait it out
                freshBuffer = false;
            }
            else
            {
                currentMoveDirection = bufferMoveDirection;
                bufferMoveDirection = Vector2.zero;
            }
        }
    }

    public void TryUpdateMovementDirection(Vector2 newDirection)
    {
        // Check if movement direction is valid
        bool validMovement = true;
        Vector2 curPos = body.GetCurrentCoordinates();
        validMovement = CheckMovementValidity(curPos + newDirection);

        // If the the new direction is valid for movement, change course
        if (validMovement)
        {
            currentMoveDirection = newDirection;
            // Nullify buffered inputs
            bufferMoveDirection = Vector2.zero;
        }
        else
        {
            // Second, check if the input would be valid for one tile further
            if (CheckMovementValidity(curPos + currentMoveDirection + newDirection))
            {
                // --> if so, stash it into the buffer
                bufferMoveDirection = newDirection;
                freshBuffer = true;
            }
        }

        // Manual movement
        //DoMove();


    }
    private bool CheckMovementValidity(Vector2 pos)
    {
        if (TileManager.Instance.IsThereTileAt(pos))
        {
            return false;
        }
        else if (body.IsThereSnakePieceAt(pos))
        {
            return false;
        }
        return true;
    }
    #endregion

    #region Edibles
    private void DoEdibleCheck()
    {
        Vector2 curPos = body.GetCurrentCoordinates();

        // Check if on top of any edibles
        if (CheckPosForEdibles(curPos))
        {
            // Eat them
            EdibleManager.Instance.EatEdibleAtPos(curPos);
            body.RegisterGrowth();
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
    #endregion

    #region Helpers
    public void ResetSnake()
    {
        currentMoveDirection = ORIGINALMoveDirection;
        body.ReturnToStartPositions();
    }
    #endregion
}
