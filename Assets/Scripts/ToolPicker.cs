using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

class ToolPicker : MonoBehaviour {
    public UITool CurrentTool { get; private set; }
    public Camera camera;
    public RectTransform backgroundPosition;
    
    [SerializeField] private Button buttonPrefab;

    private List<Button> buttonPositions = new List<Button>();

    private void Start() {
        foreach (State state in Enum.GetValues(typeof(State))) {
            if (!state.IsPlaceable())
                continue;

            Button button = Instantiate(buttonPrefab, transform);
            button.onClick.AddListener(() => HandleStateSelected(state));

            button.GetComponent<Image>().color = GridHolder.stateToColor[state];

            buttonPositions.Add(button);
        }
    }

    private void HandleStateSelected(State state) {
        Debug.Log(state);
        CurrentTool = new PlaceTileTool(state);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            CurrentTool = null;
        
        float width = camera.scaledPixelWidth;
        float height = camera.scaledPixelHeight;
        
        float c = 0.085f;
        
        backgroundPosition.sizeDelta = new Vector2(width, height * c);
        backgroundPosition.anchoredPosition = new Vector2(0, -height * c / 2);
        
        float buttonSize = backgroundPosition.sizeDelta.y * 0.7f;
        float buttonSpacing = buttonSize * 0.25f;

        int buttonCount = buttonPositions.Count;

        float toolsWidth = buttonSpacing * (buttonCount - 1) + buttonSize * buttonCount;

        float leftOffset = (width - toolsWidth) / 2;

        int buttonNum = 0;
        foreach (Button button in buttonPositions) {
            RectTransform buttonTransform = button.GetComponent<RectTransform>();

            float x = buttonNum * (buttonSize + buttonSpacing) + buttonSize / 2 + leftOffset;
            
            buttonTransform.anchoredPosition = new Vector2(x, backgroundPosition.anchoredPosition.y);
            buttonTransform.sizeDelta = new Vector2(buttonSize, buttonSize);

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