using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SnakeGraphicsCollection")]
public class SnakeGraphicsCollection : ScriptableObject
{
    // Different snakeparts are stored here

    public Sprite head_shut;
    public Sprite head_open;
    public Sprite body_straight;
    public Sprite body_turn;
    public Sprite tail;
}
