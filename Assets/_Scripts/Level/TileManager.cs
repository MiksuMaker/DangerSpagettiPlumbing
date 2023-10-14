using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoSingleton<TileManager>
{
    #region Properties
    Tilemap level;
    #endregion

    #region Setup
    private void Start()
    {
        level = FindObjectOfType<Tilemap>();
    }
    #endregion

    #region Functions
    public bool IsThereTileAt(Vector2 pos)
    {
        // Check tile at

        Tile tile = level.GetTile(new Vector3Int((int)pos.x, (int)pos.y)) as Tile;

        if (tile == null)
        { return false; }
        else
        { return true; }
    }
    #endregion
}
