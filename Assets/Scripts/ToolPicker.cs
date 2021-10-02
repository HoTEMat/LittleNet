using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor.PackageManager.UI;

class ToolPicker : MonoBehaviour {
    public UITool CurrentTool { get; private set; }
    public Camera camera;
    
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

            button.GetComponent<Image>().color = GridTile.stateToColor[state];

            buttonPositions.Add(button);

            RectTransform buttonTransform = button.GetComponent<RectTransform>();
            float x = buttonNum * (buttonTransform.rect.width + buttonSpacing) + buttonTransform.rect.width / 2 + leftOffset;
            buttonTransform.anchoredPosition = new Vector2(x, -buttonTransform.rect.height / 2 - buttonSpacing);

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
            
            float x = buttonNum * (buttonTransform.rect.width + buttonSpacing) + buttonTransform.rect.width / 2 + leftOffset;
            buttonTransform.anchoredPosition = new Vector2(x, -buttonTransform.rect.height / 2 - buttonSpacing);

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