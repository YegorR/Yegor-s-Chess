using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UCIAdapter
{
    private Process process;
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
    }

    private void SendCommand(string command)
    {
        UnityEngine.Debug.Log(command);
        process.StandardInput.WriteLine(command);
    }

    public void Start()
    {
        SendCommand("uci");
    }
}
