using System.Collections;
using System.Collections.Generic;
using HISTORY;
using UnityEngine;

public class HistoryTesting : MonoBehaviour
{
    public HistoryState state = new HistoryState();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            state = HistoryState.Capture();
    }
}
