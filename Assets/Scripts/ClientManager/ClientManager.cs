using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public GameObject boardPrefab;
    public event ActedHandler ActEvent;

    private Board board;
    private bool endGame = false;

    [SerializeField] private GameObject notifyingOpponentExitsPanel;
    [SerializeField] private GameObject notifyingDisconnectPanel;

    public void Start()
    {
        GameObject boardObject = GameObject.Instantiate(boardPrefab);
        board = boardObject.GetComponent<Board>();
        board.MoveIsMadeEvent += MoveIsMade;
    }

    private void MoveIsMade(Cell from, Cell to)
    {
        PlayerAct playerAct = new PlayerAct
        {
            Act = PlayerAct.ActType.Move,
            From = from,
            To = to
        };
        ActEvent(playerAct);
    }

    public void ChangeGraphicMode(bool is3dMode)
    {
        board.GraphicMode = is3dMode;
        Camera.main.GetComponent<CameraScript>().Is3D = is3dMode;
    }

    public void ExitButtonPressed()
    {
        if (endGame)
        {
            SceneManager.LoadScene(0);
            return;
        }
        PlayerAct playerAct = new PlayerAct()
        {
            Act = PlayerAct.ActType.Exit
        };
        ActEvent(playerAct);
        SceneManager.LoadScene(0);
    }

    public void Block(bool isBlock, PlayerColor playerColor)
    {
        board.Block(isBlock, playerColor);
    }

    public void SetGameSituation(GameSituation gameSituation)
    {
        if (gameSituation.GameStatus == GameStatus.OpponentExits)
        {
            endGame = true;
            NotifyOpponentExits();
            return;
        }
        board.SetGameSituation(gameSituation);
    }

    private void NotifyOpponentExits()
    {
        notifyingOpponentExitsPanel.SetActive(true);
    }

    public void OpponentsExitsNotified()
    {
        notifyingOpponentExitsPanel.SetActive(false);
    }

    private void NotifyDisconnect()
    {
        notifyingDisconnectPanel.SetActive(true);
    }

    public void DisconectNotified()
    {
        notifyingDisconnectPanel.SetActive(false);
    }

    public void Disconnect()
    {
        if (endGame) return;
        Block(true, PlayerColor.White);
        Block(true, PlayerColor.Black);
        NotifyDisconnect();
        return;
    }
}