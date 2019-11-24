public class PlayerAct
{
    public enum ActType
    {
        Move, OfferDraw, Surrender, Exit
    }

    public ActType Act { get; set; }

    public Cell From { get; set; }
    public Cell To { get; set; }
}

public struct SerializedPlayerAct
{
    public int act;
    public int fromVertical, fromHorizontal;
    public int toVertical, toHorizontal;
    
    public static SerializedPlayerAct Serialize(PlayerAct playerAct)
    {
        SerializedPlayerAct result = new SerializedPlayerAct
        {
            act = (int)playerAct.Act
        };
        if (playerAct.Act != PlayerAct.ActType.Move)
        {
            return result;
        }
        result.fromVertical = playerAct.From.Vertical;
        result.fromHorizontal = playerAct.From.Horizontal;
        result.toVertical = playerAct.To.Vertical;
        result.toHorizontal = playerAct.To.Horizontal;

        return result;
    }

    public static PlayerAct Deserealize(SerializedPlayerAct serialized) 
    {
        PlayerAct playerAct = new PlayerAct
        {
            Act = (PlayerAct.ActType)serialized.act
        };
        if (playerAct.Act != PlayerAct.ActType.Move)
        {
            return playerAct;
        }
        playerAct.From = new Cell(serialized.fromVertical, serialized.fromHorizontal);
        playerAct.To = new Cell(serialized.toVertical, serialized.toHorizontal);
        return playerAct;
    }
}
