using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UCIAdapter
{
    private Process process;
    private State state = State.Preload;

    public delegate void EngineMovedEventHandler(Cell from, Cell to);
    public event EngineMovedEventHandler EngineMovedEvent;

    public UCIAdapter(string enginePath)
    {
        process = new Process();
        process.EnableRaisingEvents = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.FileName = enginePath;

        process.OutputDataReceived += ReceiveProcessData;

        if (!process.Start())
        {
            throw new System.Exception("Ошибка запуска процесса шахматного движка");
        }

        process.BeginOutputReadLine();
    }

    private void ReceiveProcessData(object sender, DataReceivedEventArgs e)
    {
        UnityEngine.Debug.Log(e.Data);
        State oldState = state;

        string msg = e.Data.Trim();
        string[] msgWords = msg.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        switch (state)
        {
            case State.Start:
                if (!msg.Equals("uciok"))
                {
                    return;
                }
                SendCommand("ucinewgame");
                state = State.Ready;
                break;
            case State.Run:
                if ((msgWords.Length < 1) || (!msgWords[0].Equals("bestmove")))
                {
                    return;
                }
                if (msgWords.Length < 2)
                {
                    throw new System.Exception("Ошибка шахматного движка: некорректный bestmove");
                }
                Cell from = new Cell(msgWords[1].Substring(0, 2));
                Cell to = new Cell(msgWords[1].Substring(2, 2));
                state = State.Ready;
                EngineMovedEvent(from, to);
                break;
        }

        if (state != oldState)
        {
            UnityEngine.Debug.Log("State: " + state);
        }
    }

    private void SendCommand(string command)
    {
        UnityEngine.Debug.Log(command);
        process.StandardInput.WriteLine(command);
    }

    public void Start()
    {
        state = State.Start;
        SendCommand("uci");
    }

    public void SetGameSituation(GameSituation gameSituation)
    {
        if (state != State.Ready)
        {
            throw new System.Exception("Движок не готов к ходу");
        }
        SendCommand("position fen " + TranslateGameSituationToFen(gameSituation));
        state = State.Run;
        SendCommand("go");
    } 

    private enum State
    {
        Preload, Start, Ready, Run
    }

    private string TranslateGameSituationToFen(GameSituation gameSituation)
    {
        char GetPiece(ChessPieceType chessPieceType, PlayerColor playerColor)
        {
            char ch = ' ';
            switch (chessPieceType)
            {
                case ChessPieceType.Pawn:
                    ch = 'P';
                    break;
                case ChessPieceType.Rook:
                    ch = 'R';
                    break;
                case ChessPieceType.Knight:
                    ch = 'N';
                    break;
                case ChessPieceType.Bishop:
                    ch = 'B';
                    break;
                case ChessPieceType.Queen:
                    ch = 'Q';
                    break;
                case ChessPieceType.King:
                    ch = 'K';
                    break;
                case ChessPieceType.None:
                    return ' ';
            }
            if (playerColor == PlayerColor.Black)
            {
                ch = char.ToLower(ch);
            }
            return ch;
        }

        string fen = "";
        for (int j = 7; j >= 0; j--)
        {
            int noPieceCount = 0;
            for (int i = 0; i < 8; i++)
            {
                char piece = GetPiece(gameSituation.PiecesLocation[i, j].Item1, gameSituation.PiecesLocation[i, j].Item2);
                if (piece == ' ')
                {
                    noPieceCount++;
                }
                else
                {
                    if (noPieceCount != 0)
                    {
                        fen += noPieceCount.ToString();
                        noPieceCount = 0;
                    }
                    fen += piece;
                }
            }
            if (noPieceCount != 0)
            {
                fen += noPieceCount.ToString();
            }
        }
        fen += " ";

        if (gameSituation.IsWhiteMoving)
        {
            fen += "w ";
        }
        else
        {
            fen += "b ";
        }

        string castling = "";
        if (gameSituation.isWhiteKingCastlingPossible)
        {
            castling += "K";
        }
        if (gameSituation.isWhiteQueenCastlingPossible)
        {
            castling += "Q";
        }
        if (gameSituation.isBlackKingCastlingPossible)
        {
            castling += "k";
        }
        if (gameSituation.isBlackQueenCastlingPossible)
        {
            castling += "q";
        }
        if (castling.Equals(""))
        {
            castling = "-";
        }
        fen += castling;
        fen += " ";

        if (gameSituation.aislePawnCell != null)
        {
            fen += gameSituation.aislePawnCell.ToString() + " ";
        }
        else
        {
            fen += "- ";
        }

        fen += "1 1";   //TODO: last two field of DEN
        return fen;
    }
}
