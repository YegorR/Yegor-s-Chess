public class PlayerAct
{
    public enum ActType
    {
        Move, OfferDraw, Surrender, Exit
    }

    public ActType Act { get; set; }

    public Cell From { get; set; }
    public Cell To { get; set; }

    public IPlayer Player { get; set; }
}
