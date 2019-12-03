using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log("testscript is beginning");
        UCIAdapter uciAdapter = new UCIAdapter(Application.dataPath + "/Engine/stockfish_10_x64.exe");
        uciAdapter.Start();
    }


    // Update is called once per frame
    void Update()
    {

    }
}
