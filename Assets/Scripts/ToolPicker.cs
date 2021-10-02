using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor.PackageManager.UI;

class ToolPicker : MonoBehaviour {
    public UITool CurrentTool { get; private set; }
    public Camera camera;
    public RectTransform backgroundPosition;

    [SerializeField] private Button buttonPrefab;

    private List<Button> buttonPositions = new List<Button>();

    private int buttonSpacing = 8;
    private int buttonCount;
    private float buttonSize;

    private void Start() {
        int buttonNum = 0;
        buttonSize = buttonPrefab.GetComponent<RectTransform>().rect.width;

        buttonCount = 0;
        foreach (State state in Enum.GetValues(typeof(State))) {
            if (state.IsPlaceable())
                buttonCount += 1;
        }

        float totalWidth = buttonSpacing * (buttonCount - 1) +
                           buttonSize * buttonCount;

        float leftOffset = (camera.scaledPixelWidth - totalWidth) / 2;

        foreach (State state in Enum.GetValues(typeof(State))) {
            if (!state.IsPlaceable())
                continue;

            Button button = Instantiate(buttonPrefab, transform);
            button.onClick.AddListener(() => HandleStateSelected(state));

            button.GetComponent<Image>().color = GridHolder.stateToColor[state];

            buttonPositions.Add(button);

            RectTransform buttonTransform = button.GetComponent<RectTransform>();
            float x = buttonNum * (buttonSize + buttonSpacing) + buttonSize / 2 + leftOffset;
            buttonTransform.anchoredPosition = new Vector2(x, backgroundPosition.anchoredPosition.y - buttonSize / 2);

            buttonNum++;
        }
    }

    private void HandleStateSelected(State state) {
        Debug.Log(state);
        CurrentTool = new PlaceTileTool(state);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            CurrentTool = null;
        }

        float totalWidth = buttonSpacing * (buttonCount - 1) +
                           buttonSize * buttonCount;

        float leftOffset = (camera.scaledPixelWidth - totalWidth) / 2;

        int buttonNum = 0;
        foreach (Button button in buttonPositions) {
            RectTransform buttonTransform = button.GetComponent<RectTransform>();

            float x = buttonNum * (buttonSize + buttonSpacing) + buttonSize / 2 + leftOffset;
            buttonTransform.anchoredPosition = new Vector2(x, backgroundPosition.anchoredPosition.y);

            buttonNum++;
        }
    }
}

interface UITool {
}

class PlaceTileTool : UITool {
    public State TileType { get; }

    public PlaceTileTool(State tileType) {
        this.TileType = tileType;
    }
}