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
    #endregion

    #region Functions
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

        body.MoveSnakeBody(currentMoveDirection);
    }
    #endregion
}
