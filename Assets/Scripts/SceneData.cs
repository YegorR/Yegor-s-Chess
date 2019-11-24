using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneData
{
    public enum GameType
    {
        Host, Client, Ai, HotSeat
    }

    public static GameType Type { get; set; }

    public static PlayerColor PlayerColor { get; set; }

    public static string Host { get; set; }

}
