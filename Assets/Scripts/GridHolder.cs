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

    public bool ShowTileTextures { get; set; }

    [SerializeField]
    private float gridLineWidth = 0.01f;

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
        Level = Levels.NotLevel();
        InitGrid(Level.Grid);
    }

    private void Update() {
        UpdateGridTiles();

        if (Input.GetKeyDown(KeyCode.Space)) {
            Level.DoIteration();
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
