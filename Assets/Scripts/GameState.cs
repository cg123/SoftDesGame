using UnityEngine;
using System.Collections;

public class GameState : Singleton<GameState>
{
    protected GameState() { }

    public Player player;
    public bool inGame;
}
