using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyHandler : MonoBehaviour
{
    #region Properties
    // References
    SnakeGraphicsController graphics;

    List<BodyBlock> bodyparts = new List<BodyBlock>();
    List<(BodyBlock, Vector2)> startPositions = new List<(BodyBlock, Vector2)>();
    int growthNeeded = 0;

    bool lastMoveDirectionIsRight = true;


    [HideInInspector] public bool openMouth = false;
    [HideInInspector] public bool interruptableMovementInProgress = false;
    [SerializeField] float dropTime = 0.5f;

    IEnumerator snakeDropper;
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
        SaveStartPositions();
    }

    [ContextMenu("Get Bodyparts")]
    private void UpdateBodyparts()
    {
        // Go through all children and store them on the Bodyparts list
        for (int i = 0; i < transform.childCount; i++)
        {
            bodyparts.Add(transform.GetChild(i).GetComponent<BodyBlock>());
        }
    }

    private void SaveStartPositions()
    {
        foreach (var b in bodyparts)
        {
            startPositions.Add((b, b.transform.position));
        }
    }

    public void ReturnToStartPositions()
    {
        // Stop falling
        if (snakeDropper != null)
        { StopCoroutine(snakeDropper); }

        // Delete extra blocks?

        // Reset the blocks to starting positions
        for (int i = 0; i < startPositions.Count; i++)
        {
            bodyparts[i].transform.position = startPositions[i].Item2;
        }

        UpdateGraphics();
    }
    #endregion

    #region Moving
    public void MoveSnakeBody(Vector2 nextCoordinates)
    {
        UpdateMoveDirection(nextCoordinates.x);

        Vector2 prevBodypartPos;
        Vector2 nextPos;

        prevBodypartPos = bodyparts[0].transform.position;

        // Move the head to next position
        bodyparts[0].transform.position += new Vector3(nextCoordinates.x, nextCoordinates.y);

        // Check the need for growth
        if (growthNeeded != 0) { HandleGrowth(prevBodypartPos); }
        else
        {
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
        }

        // Update graphics
        UpdateGraphics();
    }

    private void UpdateGraphics()
    {
        for (int i = 0; i < bodyparts.Count; i++)
        {
            // Check type
            if (i == 0) // HEAD
            {
                graphics.UpdateBodypartGraphics(null, bodyparts[0], bodyparts[1], lastMoveDirectionIsRight);
                graphics.OpenMouth(bodyparts[0], openMouth);
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

    public bool IsThereSnakePieceAt(Vector2 coordinates)
    {
        // Check through all body parts if they are on the way
        for (int i = 0; i < bodyparts.Count; i++)
        {
            if (bodyparts[i].transform.position == (Vector3)coordinates)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Growing
    public void RegisterGrowth(int growthAmount = 1)
    {
        growthNeeded += growthAmount;
    }

    private void HandleGrowth(Vector2 newGrowthPos)
    {
        // Add another piece to the snake
        BodyBlock b = (Instantiate(Resources.Load("Snake/BodyPiece"), transform) as GameObject).GetComponent<BodyBlock>();

        // Arrange it to the right position
        bodyparts.Insert(1, b);

        b.transform.position = newGrowthPos;

        growthNeeded--;
    }
    #endregion

    #region Dropping
    public bool CheckAreThereTilesUnderSnake(int heightModifier = 0)
    {
        // Check if there is any ground tile supporting the snake body
        for (var i = 0; i < bodyparts.Count; i++)
        {
            if (TileManager.Instance.IsThereTileAt(bodyparts[i].transform.position
                                                   + new Vector3(0f,
                                                                 -(1f + heightModifier),
                                                                 0f)))
            {
                // If there is even one tile, the snake IS SUPPORTED
                return true;
            }
        }
        // If there are no tiles supporting, the SNAKE WILL FALL
        return false;
    }

    public void CalculateFallDistance()
    {
        // There is no ground tiles beneath the snake - time to drop them

        // Calculate the drop position
        int fallLimit = 5;
        int fallDistance = 0;
        for (int i = 0; i < fallLimit; i++)
        {
            if (CheckAreThereTilesUnderSnake(i))
            {
                break;
            }
            fallDistance++;
        }

        // Drop the snake for the FallDistance
        if (fallDistance > 0)
        {
            // Drop the snake bits for that distance
            DropSnakeDown(fallDistance);
        }

    }

    public void DropSnakeDown(int fallDistance)
    {
        //
        snakeDropper = DropperCoroutine(fallDistance);
        StartCoroutine(snakeDropper);
    }

    IEnumerator DropperCoroutine(int fallDistance)
    {
        PauseMovement();

        Vector3 fall = Vector3.down * fallDistance;

        // Pack the BodyBlocks in the tuple
        List<(BodyBlock, Vector3, Vector3)> instructions = new List<(BodyBlock, Vector3, Vector3)>();
        foreach (var b in bodyparts)
        {
            Vector3 pos = b.transform.position;
            instructions.Add((b, pos, pos + fall));
        }

        // Drop each to the desired position
        float dropTime = this.dropTime;
        float elapsedTime = 0f;
        while (elapsedTime < dropTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            elapsedTime += Time.deltaTime;

            float t = Easing.EaseOutBounce(elapsedTime / dropTime); // Easing

            // Calculate the current fall positions
            for (int i = 0; i < instructions.Count; i++)
            {
                instructions[i].Item1.transform.position =
                    Vector3.Lerp(instructions[i].Item2,
                                 instructions[i].Item3,
                                 t);
            }
        }

        // Finalize positions
        for (int i = 0; i < instructions.Count - 1; i++)
        {
            instructions[i].Item1.transform.position =
                instructions[i].Item3;
        }

        ResumeMovement();
    }

    private void PauseMovement()
    {
        interruptableMovementInProgress = true;
        MovementManager.Instance.pauseMovementTick();
    }

    private void ResumeMovement()
    {
        interruptableMovementInProgress = false;
        MovementManager.Instance.resumeMovementTick();
    }
    #endregion

    #region Helpers
    public List<BodyBlock> GetBodyBlocks()
    {
        return bodyparts;
    }
    #endregion
}
