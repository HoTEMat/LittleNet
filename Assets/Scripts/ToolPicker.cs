using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ToolPicker : MonoBehaviour
{
    public UITool CurrentTool { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) {
            CurrentTool = UITool.PlaceWire;
        }
    }
}

enum UITool {
    None,
    PlaceWire
}
