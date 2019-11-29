using UnityEngine;
using System.Collections;

public class HotSeatFactory
{
    public HotSeatFactory()
    {
        HotSeatPlayer whitePlayer = new HotSeatPlayer();
        HotSeatPlayer blackPlayer = whitePlayer;
        ServerManager serverManager = new ServerManager(whitePlayer, blackPlayer);
    }
}
