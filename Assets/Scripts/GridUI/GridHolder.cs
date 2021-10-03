using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.ObjectModel;

class GridHolder : MonoBehaviour, ISerializationCallbackReceiver {
    public GridTile GridTilePrefab;
    public GameObject GridLinePrefab;
    public ToolPicker ToolPicker;
    public EventSystem EventSystem;
    public PlayManager PlayManager;

    private int playSpeed = 10;
    private int playCount = 0;
    
    
    public bool ShowTileTextures { get; set; }

    [SerializeField]
    private float gridLineWidth = 0.01f;

    public GameObject GridBackgroundPrefab;
    private List<GameObject> gridBackgrounds = new List<GameObject>();

    public ILevel Level { get; private set; }
    private GridTile[,] gridTiles;
    private UITool activeTool;

    [Serializable]
    public struct StateToSpriteItem {
        public State State;
        public Sprite Color;
    }
    [SerializeField] private StateToSpriteItem[] StateToSpriteArray;
    public IReadOnlyDictionary<State, Sprite> StateToSprite;
    public void OnBeforeSerialize() {
        // Do nothing.
    }
    public void OnAfterDeserialize() {
        var StateToSpriteMutable = new Dictionary<State, Sprite>();
        foreach (StateToSpriteItem stateSprite in StateToSpriteArray) {
            StateToSpriteMutable[stateSprite.State] = stateSprite.Color;
        }
        StateToSprite = new ReadOnlyDictionary<State, Sprite>(StateToSpriteMutable);
    }

    private void Start() {
        Level = Levels.CrossLevel();
        InitGrid(Level.Grid);
    }

    private void Update() {
        UpdateGridTiles();

        if (PlayManager.State == PlayState.Playing) {
            playCount++;

            if (playCount % playSpeed == 0)
                Level.DoIteration();
        }
        
        if (PlayManager.State == PlayState.Stopped && Level.GetIteration != 0)
            Level.Reset();
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            Level.DoIteration();
            PlayManager.State = PlayState.Paused;
        }
        
        if (!Input.GetMouseButton(0)) {
            activeTool = null;
        }
    }

    private void InitGrid(SimulationGrid grid) {
        gridTiles = new GridTile[grid.Width, grid.Height];
        InitGridTiles();
    }

    private void InitGridTiles() {
        var clickHandler = new Action<GridTile>(HandleTileClicked);
        var mouseOverTileHandler = new Action<GridTile>(HandleMouseOverTile);
        for (int gridX = 0; gridX < Level.Grid.Width; gridX++) {
            for (int gridY = 0; gridY < Level.Grid.Height; gridY++) {
                GridTile tile = Instantiate<GridTile>(GridTilePrefab, transform);
                tile.X = gridX;
                tile.Y = gridY;
                tile.SetTopLeft(GridPositionToPosition(gridX, gridY, 0));
                tile.OnClicked += clickHandler;
                tile.OnMouseInside += mouseOverTileHandler;
                gridTiles[gridX, gridY] = tile;

                IPort port = Level.Grid.GetPortAt(tile.X, tile.Y);
                if (port != null)
                {
                    var background = Instantiate(GridBackgroundPrefab, transform);
                    var position = background.GetComponent<RectTransform>();
                    
                    if (port is InputPort) background.GetComponent<SpriteRenderer>().color = Color.green;
                    else if (port is OutputPort) background.GetComponent<SpriteRenderer>().color = Color.red;
                    else background.GetComponent<SpriteRenderer>().color = Color.blue;

                    position.transform.position = tile.transform.position;
                    
                    float c = 1.15f;
                    position.localScale = new Vector3(c, c, c);
                    
                    gridBackgrounds.Add(background);
                }
            }
        }
    }

    private void UpdateGridTiles() {
        for (int gridX = 0; gridX < Level.Grid.Width; gridX++) {
            for (int gridY = 0; gridY < Level.Grid.Height; gridY++) {
                State state = Level.Grid.Get(gridX, gridY);
                GridTile tile = gridTiles[gridX, gridY];
                if (ShowTileTextures) {
                    tile.ShowSprite(StateToSprite[state]);
                } else {
                    tile.ShowColor(state);
                }
            }
        }
    }

    private void HandleTileClicked(GridTile tile) {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (ToolPicker.CurrentTool == null)
            return;
        activeTool = ToolPicker.CurrentTool;
        ApplyTool(activeTool, tile);
    }
    private void HandleMouseOverTile(GridTile tile) {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (activeTool != null) {
            ApplyTool(activeTool, tile);
        }
    }

    private void ApplyTool(UITool tool, GridTile tile) {
        if (ToolPicker.CurrentTool is PlaceTileTool placeTile) {
            Level.Grid.Set(tile.X, tile.Y, placeTile.TileType);
        }
    }

    private Vector3 GridPositionToPosition(int gridX, int gridY, float z) {
        (float w, float h) = GetTileSize();
        return new Vector3((w + gridLineWidth) * gridX, -(h + gridLineWidth) * gridY, z);
    }


    public (float width, float height) GetTileSize() {
        Vector3 tileScale = GridTilePrefab.transform.localScale;
        return (tileScale.x, tileScale.y);
    }

    public (float, float) GetGridSize() {
        (float tileWidth, float tileHeight) = GetTileSize();
        return (Level.Grid.Width * (tileWidth + gridLineWidth), Level.Grid.Height * (tileHeight + gridLineWidth));
    }

    public Vector3 GetGridCenter() {
        (float w, float h) = GetGridSize();
        return new Vector3(w / 2, -h / 2, 0);
    }
}
