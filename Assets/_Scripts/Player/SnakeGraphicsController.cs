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

    #region Update Bodyparts
    public void UpdateBodypartGraphics(BodyBlock headward, BodyBlock current, BodyBlock tailward, bool rightWard = true)
    {
        // Change the CURRENT graphics to fit the situation
        // depending on the relation to PREVIOUS and NEXT piece

        Piece piece = new Piece();

        // If previous or next piece is NULL, it is head or tails
        if (headward == null)
        {
            piece.type = Piece.Type.head;
            current.spriteRenderer.sprite = graphics.head_open;
        }
        else if (tailward == null)
        {
            piece.type = Piece.Type.tail;
            current.spriteRenderer.sprite = graphics.tail;
        }
        else
        {
            piece.type = Piece.Type.body;
            current.spriteRenderer.sprite = graphics.body_straight;
        }

        // TODO: Get Orientation + Handle GameObject retrieval properly
        piece.orientation = GetOrientation(headward, current, tailward);

        //Debug.Log("Orientation for: _" + current.name + "_ is " + piece.orientation);

        // Determine the rotation for the piece
        int rotation = 0;
        int scaleX = 1;
        int scaleY = 1;
        bool turnPiece = false;
        switch (piece.type, piece.orientation)
        {
            // BODY, STRAIGHT
            case (Piece.Type.body, Piece.Orientation.left): rotation = 0; break;
            case (Piece.Type.body, Piece.Orientation.right): rotation = 0; break;
            case (Piece.Type.body, Piece.Orientation.up): rotation = 90; break;
            case (Piece.Type.body, Piece.Orientation.down): rotation = 90; break;

            // BODY, TURNED (Update graphics too)
            case (Piece.Type.body, Piece.Orientation.left_up): rotation = 0; turnPiece = true; break;
            case (Piece.Type.body, Piece.Orientation.left_down): rotation = 90; turnPiece = true; break;
            case (Piece.Type.body, Piece.Orientation.right_up): rotation = -90; turnPiece = true; break;
            case (Piece.Type.body, Piece.Orientation.right_down): rotation = 180; turnPiece = true; break;

            // HEAD
            case (Piece.Type.head, Piece.Orientation.left): rotation = 180; scaleY = CalcScale(rightWard); break;
            case (Piece.Type.head, Piece.Orientation.right): rotation = 0; scaleY = CalcScale(rightWard); break;
            case (Piece.Type.head, Piece.Orientation.up): rotation = 90; scaleY = CalcScale(rightWard); break;
            case (Piece.Type.head, Piece.Orientation.down): rotation = -90; scaleY = CalcScale(rightWard); break;

            // TAIL
            case (Piece.Type.tail, Piece.Orientation.left): rotation = 180; break;
            case (Piece.Type.tail, Piece.Orientation.right): rotation = 0; break;
            case (Piece.Type.tail, Piece.Orientation.up): rotation = 90; break;
            case (Piece.Type.tail, Piece.Orientation.down): rotation = -90; break;
        }

        // If it was a turning piece, change graphics
        if (turnPiece) { current.spriteRenderer.sprite = graphics.body_turn; }

        // Rotate the bodypiece graphics
        current.graphicsTransform.rotation = Quaternion.Euler(0f, 0f, rotation);
        current.graphicsTransform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    private Piece.Orientation GetOrientation(BodyBlock headward, BodyBlock current, BodyBlock tailward)
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

                return CalculateOrientation(tailward.transform.position, currentPos, true);

            case (false, true): // Bodypiece is TAILS

                return CalculateOrientation(headward.transform.position, currentPos, false);

            default: return Piece.Orientation.left; // Default
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

    private Piece.Orientation CalculateOrientation(Vector2 otherPos, Vector2 curPos, bool isHead)
    {
        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;

        if (otherPos.x < curPos.x) { left = true; }
        if (otherPos.x > curPos.x) { right = true; }

        if (otherPos.y > curPos.y) { up = true; }
        if (otherPos.y < curPos.y) { down = true; }

        // Get orientation from the combination
        switch (left, right, up, down)
        {
            case (true, false, false, false):    // LEFT
                return isHead ? Piece.Orientation.right : Piece.Orientation.left;

            case (false, true, false, false):    // RIGHT
                return isHead ? Piece.Orientation.left : Piece.Orientation.right;

            case (false, false, true, false):    // UP
                return isHead ? Piece.Orientation.down : Piece.Orientation.up;

            case (false, false, false, true):    // DOWN
                return isHead ? Piece.Orientation.up : Piece.Orientation.down;


            default:
                return Piece.Orientation.left;
        }
    }
    #endregion

    #region Helpers
    private int CalcScale(bool rightwards)
    {
        if (rightwards) { return 1; }
        // Else
        return -1;
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