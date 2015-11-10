using UnityEngine;
using System.Collections.Generic;
public enum Fraction
{
    Neutral, Red,Blue
}
public class PlayerInfo
{
    static int uuid = 0;
    public string name;
    public int id
    {
        get;
        internal set;
    }
    public Fraction fraction;
    public PlayerInfo(string name)
    {
        this.name = name;
        this.id = uuid++;
    }
    public PlayerInfo()
    {
        this.id = uuid++;
        this.name = "玩家"+uuid;

    }
    public PlayerInfo(Fraction fraction)
    {
        this.id = uuid++;
        this.name = "玩家" + uuid;
        this.fraction = fraction;

    }
    public PlayerInfo(string name, Fraction fraction)
    {
        this.name = name;
        this.id = uuid++;
        this.fraction = fraction;
    }
}
public enum Player
{

    Neutral, Player1, Player2, 

}
public static class PlayerManager{
    static PlayerInfo[] playerInfoList = new PlayerInfo[10];
    public static Player localPlayer;
    public static void init()
    {
        localPlayer = Player.Player1;
        playerInfoList[0] = new PlayerInfo("中立",Fraction.Blue);
        playerInfoList[1] = new PlayerInfo(Fraction.Blue);
        playerInfoList[2] = new PlayerInfo(Fraction.Red);
      
    }
    public static bool isOpponent(Player one, Player two)
    {
        return playerInfoList[(int)two].fraction != Fraction.Neutral && playerInfoList[(int)one].fraction != playerInfoList[(int)two].fraction;
    }
    public static bool isFriend(Player one, Player two)
    {
        return playerInfoList[(int)two].fraction != Fraction.Neutral && playerInfoList[(int)one].fraction == playerInfoList[(int)two].fraction;
    }
    public static bool isNeutral(Player player)
    {
        return playerInfoList[(int)player].fraction == Fraction.Neutral;
    }
    public static PlayerInfo getPlayerInfo(Player player)
    {
        return playerInfoList[(int)player];
    }

}

