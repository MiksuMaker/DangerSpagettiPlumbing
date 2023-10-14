using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoSingleton<MovementManager>
{
    #region Properties
    // Delegates and Events
    public delegate void PauseMovementTick();
    public PauseMovementTick pauseMovementTick;
    public delegate void ResumeMovementTick();
    public ResumeMovementTick resumeMovementTick;
    public delegate void MovementTick();
    public event MovementTick movementTick;

    [SerializeField, Range(0f, 5f)] float movementFrequency = 1f;
    [SerializeField] bool pausedMovement = false;
    [SerializeField] bool automaticMovementOn = true;
    #endregion

    #region Setup
    private void Start()
    {
        pauseMovementTick += Pause;
        resumeMovementTick += Resume;
        StartCoroutine(MoveTicker());
    }
    #endregion

    #region Movement Tick
    IEnumerator MoveTicker()
    {
        float currentTimeTillTick = movementFrequency;
        while (true)
        {
            yield return Time.deltaTime;

            if (!pausedMovement && automaticMovementOn)
            {
                currentTimeTillTick -= Time.deltaTime;
            }

            if (currentTimeTillTick <= 0)
            {
                // Tick
                movementTick?.Invoke();

                // Reset timer
                currentTimeTillTick = movementFrequency;
            }
        }
    }

    private void Pause()
    {
        pausedMovement = true;
    }

    private void Resume()
    {
        pausedMovement = false;
    }
    #endregion
}
