using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SnakeGraphicsCollection")]
public class SnakeGraphicsCollection : ScriptableObject
{
    // Different snakeparts are stored here

    [SerializeField] Sprite head_shut;
    [SerializeField] Sprite head_open;
    [SerializeField] Sprite head_body_straight;
    [SerializeField] Sprite head_body_turn;
    [SerializeField] Sprite head_tail;
}
