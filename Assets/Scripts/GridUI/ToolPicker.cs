using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

class ToolPicker : MonoBehaviour {
    public UITool CurrentTool { get; private set; }
    public Camera camera;
    public RectTransform backgroundPosition;
    public RectTransform backgroundBorderPosition;
    public GridHolder GridHolder;

    public GameObject IterationCounter;
    private Text IterationText;
    private RectTransform IterationCounterTransform;
    
    public GameObject toolPickerBackgroundPrefab;
    private GameObject toolPickerBackground;
    
    [SerializeField] private Button buttonPrefab;

    private List<Button> buttonPositions = new List<Button>();

    private void Start() {
        toolPickerBackground = Instantiate(toolPickerBackgroundPrefab, transform);

        IterationText = IterationCounter.GetComponent<Text>();
        IterationCounterTransform = IterationCounter.GetComponent<RectTransform>();
        
        foreach (State state in Enum.GetValues(typeof(State))) {
            if (!state.IsPlaceable())
                continue;

            Button button = Instantiate(buttonPrefab, transform);
            button.onClick.AddListener(() => HandleStateSelected(state));

            button.GetComponent<Image>().sprite = GridHolder.StateToSprite[state];

            buttonPositions.Add(button);
        }
    }

    private void HandleStateSelected(State state) {
        CurrentTool = new PlaceTileTool(state);
        
        int i = 0;
        foreach (State s in Enum.GetValues(typeof(State))) {
            if (!s.IsPlaceable())
                continue;

            if (s == state) {
                var t = toolPickerBackground.GetComponent<RectTransform>();
                var m = buttonPositions[i].GetComponent<RectTransform>();

                t.sizeDelta = m.sizeDelta * 1.25f;
                t.anchoredPosition = m.anchoredPosition;
                break;
            }

            i++;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            CurrentTool = null;
        }
        
        if (CurrentTool == null)
            toolPickerBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

        float width = camera.scaledPixelWidth;
        float height = camera.scaledPixelHeight;
        
        float c = 0.085f;
        float d = 1.1f;
        
        backgroundPosition.sizeDelta = new Vector2(0, height * c / d);
        backgroundPosition.anchoredPosition = new Vector2(0, -height * c / 2);
        
        backgroundBorderPosition.sizeDelta = new Vector2(0, height * c);
        backgroundBorderPosition.anchoredPosition = new Vector2(0, -height * c / 2);
        
        IterationCounterTransform.sizeDelta = new Vector2(0, height * c / d);
        
        float buttonSize = backgroundPosition.sizeDelta.y * 0.85f;
        
        float rightOffset = -(backgroundPosition.anchoredPosition.y + buttonSize) / 2;
        IterationCounterTransform.anchoredPosition = new Vector2(-rightOffset, -height * c / 2);
        
        buttonSize = backgroundPosition.sizeDelta.y * 0.7f;
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