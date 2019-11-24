using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MenuCotroller : MonoBehaviour
{
    private State state = State.Main;
    private ISet<GameObject> buttons;

    [SerializeField] private GameObject multiplayerButton;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject clientButton;
    [SerializeField] private GameObject cancelMultiplayerButton;
    [SerializeField] private GameObject colorChoice;
    [SerializeField] private GameObject startHostButton;
    [SerializeField] private GameObject cancelHostButton;
    [SerializeField] private GameObject inputHostField;
    [SerializeField] private GameObject connectButton;
    [SerializeField] private GameObject cancelClientButton;

    private PlayerColor playerColor = PlayerColor.White;
    private string host = "127.0.0.1";

    public void Start()
    {
        buttons =  new HashSet<GameObject>();
        HandleState();
    }

    public void ClickMultiplayerButton()
    {
        state = State.Multiplayer;
        HandleState();
    }

    public void ClickExitButton()
    {
        Application.Quit();
    }

    public void ClickCancel()
    {
        if (state == State.Multiplayer)
        {
            state = State.Main;
        }
        else if (state == State.Host)
        {
            state = State.Multiplayer;
            playerColor = PlayerColor.White;
        }
        else if (state == State.Client)
        {
            state = State.Multiplayer;
            host = "127.0.0.1";
        }
        HandleState();
    }

    public void ChangeColor(int color)
    {
        if (color == 0)
        {
            playerColor = PlayerColor.White;
        }
        else if (color == 1)
        {
            playerColor = PlayerColor.Black;
        }
        else if (color == 2)
        {
            playerColor = PlayerColor.None;
        }
    }
    public void ClickStartHostButton()
    {
        SceneData.Type = SceneData.GameType.Host;
        SceneData.PlayerColor = playerColor;
        SceneManager.LoadScene(1);
    }

    public void ClickHostButton()
    {
        state = State.Host;
        HandleState();
    }

    public void ClickClientButton()
    {
        state = State.Client;
        HandleState();
    }

    public void OnHostChanged(string host)
    {
        this.host = host;
    }

    public void ClickConnectButton()
    {
        SceneData.Type = SceneData.GameType.Client;
        SceneData.Host = host;
        SceneManager.LoadScene(1);

        //GameObject.Find("ChessNetworkManager").GetComponent<NetworkManager>().networkAddress = host;
        //NetFactory netFactory = new NetFactory();
        //netFactory.Initialize(false, PlayerColor.None);
    }

    private void HandleState()
    {
        void ActivateComponent(GameObject obj)
        {
            obj.SetActive(true);
            buttons.Add(obj);
        }

        foreach (GameObject obj in buttons)
        {
            obj.SetActive(false);
        }
        buttons.Clear();
        switch (state)
        {
            case State.Main:
                ActivateComponent(multiplayerButton);
                ActivateComponent(exitButton);
                break;
            case State.Multiplayer:
                ActivateComponent(hostButton);
                ActivateComponent(clientButton);
                ActivateComponent(cancelMultiplayerButton);
                break;
            case State.Host:
                ActivateComponent(colorChoice);
                ActivateComponent(startHostButton);
                ActivateComponent(cancelHostButton);
                break;
            case State.Client:
                ActivateComponent(inputHostField);
                ActivateComponent(connectButton);
                ActivateComponent(cancelClientButton);
                break;
        }
    }
}

enum State
{
    Main, Multiplayer, Host, Client
}
