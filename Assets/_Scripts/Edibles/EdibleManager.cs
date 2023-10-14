using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdibleManager : MonoSingleton<EdibleManager>
{
    #region Properties
    List<Edible> edibles = new List<Edible>();
    #endregion

    #region Setup
    public void RegisterEdible(Edible e)
    {
        edibles.Add(e);
    }

    public void RemoveEdible(Edible e)
    {
        edibles.Remove(e);
    }
    #endregion

    #region Functions

    public bool CheckPositionForEdibles(Vector2 pos)
    {
        for (int i = 0; i < edibles.Count; i++)
        {
            if (edibles[i].transform.position == (Vector3)pos)
            {
                // Get eaten
                edibles[i].GetEaten();

                return true;
            }
        }

        return false;
    }
    #endregion
}
