using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyHandler : MonoBehaviour
{
    #region Properties
    // References
    SnakeGraphicsController graphics;

    [SerializeField]
    //List<GameObject> bodyparts = new List<GameObject>();
    List<BodyBlock> bodyparts = new List<BodyBlock>();

    bool lastMoveDirectionIsRight = true;
    #endregion

    #region Setup
    private void Awake()
    {
        graphics = GetComponent<SnakeGraphicsController>();
    }

    private void Start()
    {
        // Collect the bodyparts
        UpdateBodyparts();
    }

    [ContextMenu("Get Bodyparts")]
    private void UpdateBodyparts()
    {
        // Go through all children and store them on the Bodyparts list
        for (int i = 0; i < transform.childCount; i++)
        {
            bodyparts.Add(transform.GetChild(i).GetComponent<BodyBlock>());
            //Debug.Log("Added bodypart: " + transform.GetChild(i).name);
        }
    }
    #endregion

    #region Functions
    public void MoveSnakeBody(Vector2 nextCoordinates)
    {
        UpdateMoveDirection(nextCoordinates.x);

        Vector2 prevBodypartPos;
        Vector2 nextPos;

        prevBodypartPos = bodyparts[0].transform.position;

        // Move the head to next position
        bodyparts[0].transform.position += new Vector3(nextCoordinates.x, nextCoordinates.y);

        // Go through the list of bodyparts and move them along
        for (int i = 1; i < bodyparts.Count; i++)
        {
            // Get the next Position
            nextPos = prevBodypartPos;

            // Store the previous position for the next bodypart
            prevBodypartPos = bodyparts[i].transform.position;

            // Move this part forwards
            bodyparts[i].transform.position = nextPos;

        }

        // Update graphics
        for (int i = 0; i < bodyparts.Count; i++)
        {
            // Check type
            if (i == 0) // HEAD
            {
                graphics.UpdateBodypartGraphics(null, bodyparts[0], bodyparts[1], lastMoveDirectionIsRight);
            }
            else if (i != bodyparts.Count - 1) // BODY
            {
                graphics.UpdateBodypartGraphics(bodyparts[i - 1], bodyparts[i], bodyparts[i + 1], lastMoveDirectionIsRight);
            }
            else // if it is TAIL
            {
                graphics.UpdateBodypartGraphics(bodyparts[i - 1], bodyparts[i], null, lastMoveDirectionIsRight);
            }
        }
    }

    private void UpdateMoveDirection(float xInput)
    {
        if (xInput < 0)
        {
            lastMoveDirectionIsRight = false;
        }
        else if (xInput > 0)
        {
            lastMoveDirectionIsRight = true;
        }
    }

    public Vector2 GetCurrentCoordinates()
    {
        return bodyparts[0].transform.position;
    }
    #endregion
}
