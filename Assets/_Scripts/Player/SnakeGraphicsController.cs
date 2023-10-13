using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SnakeGraphicsController : MonoBehaviour
{
    #region Properties
    [SerializeField]
    SnakeGraphicsCollection graphics;

    
    #endregion

    #region Setup

    #endregion

    #region Functions
    public void UpdateBodypartGraphics(GameObject headward, GameObject current, GameObject tailward, bool rightWard = true)
    {
        // Change the CURRENT graphics to fit the situation
        // depending on the relation to PREVIOUS and NEXT piece

        Piece piece = new Piece();

        // If previous or next piece is NULL, it is head or tails
        if (headward == null)
        {
            piece.type = Piece.Type.head;
        }
        else if (tailward == null)
        {
            piece.type = Piece.Type.tail;
        }
        else
        {
            piece.type = Piece.Type.tail;
        }

        // TODO: Get Orientation + Handle GameObject retrieval properly
    }

    private Piece.Orientation GetOrientation(GameObject headward, GameObject current, GameObject tailward)
    {
        Vector2 currentPos = current.gameObject.transform.position;
        Vector2 to_pos;
        Vector2 from_pos;
        bool noForward = false;
        bool noBehind = false;

        if (headward == null)
        { noForward = true; }
        else if (tailward == null)
        { noBehind = true; }

        switch (noForward, noBehind)
        {
            case (false, false): // Bodypiece is NOT heads or tails

                return CalculateOrientation(headward.transform.position,
                                            current.transform.position,
                                            tailward.transform.position);

                break;

            case (true, false): // Bodypiece is HEAD

                Vector2 fakeHeadPos = current.transform.position + (current.transform.position - tailward.transform.position);

                return CalculateOrientation(fakeHeadPos,
                                            current.transform.position,
                                            tailward.transform.position);

                break;

            case (false, true): // Bodypiece is TAILS

                Vector2 fakeTailPos = current.transform.position + (current.transform.position - headward.transform.position);

                return CalculateOrientation(headward.transform.position,
                                            current.transform.position,
                                            fakeTailPos);

                break;

            default: return Piece.Orientation.left; 
        }
    }

    private Piece.Orientation CalculateOrientation(Vector2 forwardPos, Vector2 curPos, Vector2 behindPos)
    {
        // Check which positions have neighboring bodypart

        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;

        if (forwardPos.x < curPos.x || behindPos.x < curPos.x) { left = true; }
        if (forwardPos.x > curPos.x || behindPos.x > curPos.x) { right = true; }

        if (forwardPos.y > curPos.y || behindPos.y > curPos.y) { up = true; }
        if (forwardPos.y < curPos.y || behindPos.y < curPos.y) { down = true; }

        // Get orientation from the combination
        switch (left, right, up, down)
        {
            case (true, true, false, false):    // RIGHT
                return Piece.Orientation.right;

            case (false, false, true, true):    // UP
                return Piece.Orientation.up;

            // TURNED ORIENTATIONS

            case (true, false, true, false):    // LEFT-UP
                return Piece.Orientation.left_up;

            case (true, false, false, true):    // LEFT-DOWN
                return Piece.Orientation.left_down;

            case (false, true, true, false):    // RIGHT-UP
                return Piece.Orientation.right_up;

            case (false, true, false, true):    // RIGHT-UP
                return Piece.Orientation.right_down;

            default:
                return Piece.Orientation.left;
        }
    }
    #endregion
}

public struct Piece
{
    public Orientation orientation;
    public Type type;

    public enum Orientation
    {
        up, left, down, right,

        left_up, right_up, left_down, right_down,  
    }

    public enum Type
    {
        head, body, tail
    }
}