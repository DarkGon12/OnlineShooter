using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour, IGameState
{
    [SerializeField] public int PlayerCount { get; set; }
    [SerializeField] public string PlayerName { get; set; }
    [SerializeField] public List<string> PlayerNames { get; } = new List<string>();
}
