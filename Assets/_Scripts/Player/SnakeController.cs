using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    #region Properties
    // References
    SnakeMover mover;
    #endregion

    #region Setup
    private void Start()
    {
        mover = GetComponent<SnakeMover>();
    }
    #endregion

    #region Functions
    public void UpdateMovementDirection(Vector2 newDirection)
    {
        mover.TryUpdateMovementDirection(newDirection);
    }
    #endregion
}
