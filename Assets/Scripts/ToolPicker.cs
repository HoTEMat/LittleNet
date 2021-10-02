using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

class ToolPicker : MonoBehaviour
{
    public UITool CurrentTool { get; private set; }
    [SerializeField] private Button buttonPrefab;

    private void Start() {
        int buttonNum = 0;
        var handleStateSelected = new Action<State>(HandleStateSelected);
        foreach (State state in Enum.GetValues(typeof(State))) {
            Button button = Instantiate(buttonPrefab, transform);
            button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => HandleStateSelected(state)));

            RectTransform buttonTransform = button.GetComponent<RectTransform>();
            float x = 50 + buttonNum * (buttonTransform.rect.width + 10);
            buttonTransform.anchoredPosition = new Vector2(x, -(5 + buttonTransform.rect.height / 2));

            buttonNum++;
        }
    }

    private void HandleStateSelected(State state) {
        Debug.Log(state);
        CurrentTool = new PlaceTileTool(state);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            CurrentTool = null;
        }
    }
}

interface UITool { }

class PlaceTileTool : UITool {
    public State TileType { get; }

    public PlaceTileTool(State tileType) {
        this.TileType = tileType;
    }
}
