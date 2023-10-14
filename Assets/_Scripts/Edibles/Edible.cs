using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edible : MonoBehaviour
{
    private void Start()
    {
        // Register Edible to the manager
        EdibleManager.Instance.RegisterEdible(this);
    }

    public void GetEaten() 
    {
        EdibleManager.Instance.RemoveEdible(this);

        // Change this to pooling later
        Destroy(gameObject);
    }
}
