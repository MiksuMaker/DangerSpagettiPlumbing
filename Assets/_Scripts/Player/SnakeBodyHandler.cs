using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyHandler : MonoBehaviour
{
    #region Properties
    [SerializeField]
    List<GameObject> bodyparts = new List<GameObject>();

    #endregion

    #region Setup
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
            bodyparts.Add(transform.GetChild(i).gameObject);
            //Debug.Log("Added bodypart: " + transform.GetChild(i).name);
        }
    }
    #endregion

    #region Functions
    public void MoveSnakeBody(Vector2 nextCoordinates)
    {
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

            Debug.Log(nextPos);
        }
    }
    #endregion
}
